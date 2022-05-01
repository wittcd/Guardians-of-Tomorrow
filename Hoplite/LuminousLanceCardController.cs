using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Hoplite
{

	public class LuminousLanceCardController : CardController
	{

		public LuminousLanceCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}


		public override IEnumerator UsePower(int index = 0) 
		{
			int numTargets = GetPowerNumeral(0, 1);
			int hopliteDamage = GetPowerNumeral(1, 3);
			int targetDamage = GetPowerNumeral(2, 1);

			List<SelectCardDecision> targets = new List<SelectCardDecision>();
			IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), hopliteDamage, DamageType.Energy, numTargets, false, 1, storedResultsDecisions: targets, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (targets.FirstOrDefault() != null) 
			{
				foreach (SelectTargetDecision t in targets) 
				{
					coroutine = DealDamage(t.SelectedCard, base.CharacterCard, targetDamage, DamageType.Melee, cardSource: GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
				}
			}
		}

	}
}