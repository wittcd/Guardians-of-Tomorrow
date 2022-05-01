using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class SwarmingSplintersCardController : CardController
	{
		public SwarmingSplintersCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator UsePower(int index = 0)
		{
			int powerNumeralTargets = GetPowerNumeral(0, 1);
			int powerNumeralDamage = GetPowerNumeral(1, 1);
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), powerNumeralDamage, DamageType.Melee, powerNumeralTargets, optional: false, 0, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = SelectAndPlayCardsFromHand(DecisionMaker, 1, optional: false, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("stone", false, false), "stone"), requiredDecisions: 0);
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