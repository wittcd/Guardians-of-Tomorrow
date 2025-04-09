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
			List<Function> options = new List<Function>();
			options.Add(new Function(DecisionMaker, "Play a Stone", SelectionType.PlayCard, () => SelectAndPlayCardsFromHand(DecisionMaker, 1, optional: false, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("stone", false, false), "stone"), requiredDecisions: 0)));
			options.Add(new Function(DecisionMaker, "Put a Stone from your trash into your hand", SelectionType.DrawCard, () => GameController.SelectAndMoveCard(DecisionMaker, (Card c) => c.Location == TurnTaker.Trash && c.DoKeywordsContain("stone"), TurnTaker.ToHero().Hand, false, true, true, true, null, GetCardSource())));
			coroutine = SelectAndPerformFunction(DecisionMaker, options);
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