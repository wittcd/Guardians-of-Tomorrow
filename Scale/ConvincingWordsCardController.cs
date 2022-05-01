using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class ConvincingWordsCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public ConvincingWordsCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//one villain target deals 2 self-psychic damage.
			List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
			List<DealDamageAction> storedDamage = new List<DealDamageAction>();
			IEnumerator coroutine = GameController.SelectTargetsToDealDamageToSelf(DecisionMaker, 2, DamageType.Psychic, 1, false, 1, false, false, false, (Card c) => c.IsVillain, storedDecision, storedDamage, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//if no damage taken, deals 4 melee to another villain target
			if (storedDecision.FirstOrDefault() != null && !DidDealDamage(storedDamage)) 
			{
				coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, storedDecision.FirstOrDefault().SelectedCard), 4, DamageType.Melee, 1, false, 1, false, false, false, (Card c) => c.IsVillain, cardSource: GetCardSource());
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