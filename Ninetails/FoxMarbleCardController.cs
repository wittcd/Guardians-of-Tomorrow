using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class FoxMarbleCardController : CardController
	{
		public FoxMarbleCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator UsePower(int index = 0)
        {
			//"{Ninetails} deals 1 target 1 fire damage. You may draw 1 card or play 1 ongoing card."
			int powerNumeralTargets = GetPowerNumeral(0, 1);
			int powerNumeralDamage = GetPowerNumeral(1, 1);
			int powerNumeralDraw = GetPowerNumeral(2, 1);
			int powerNumeralOng = GetPowerNumeral(3, 1);
			DamageSource ds = new DamageSource(base.GameController, base.CharacterCard);
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, ds, powerNumeralDamage, DamageType.Fire, powerNumeralTargets, optional: false, powerNumeralTargets, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			List<Function> list = new List<Function>();
			Function item = new Function(DecisionMaker, "Draw " + powerNumeralDraw + " card", SelectionType.DrawCard, () => DrawCards(DecisionMaker, powerNumeralDraw), null, null, "Draw 1 card");
			list.Add(item);
			Function item2 = new Function(DecisionMaker, "Play " + powerNumeralOng + " ongoing card", SelectionType.PlayCard, () => SelectAndPlayCardsFromHand(DecisionMaker, powerNumeralOng, optional: false, cardCriteria: new LinqCardCriteria((Card c) => c.IsOngoing, "ongoing"), requiredDecisions: 0), base.HeroTurnTaker.Hand.Cards.Where((Card c) => c.IsOngoing).Count() > 0, null, "Play 1 ongoing card");
			list.Add(item2);
			SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, DecisionMaker, list, optional: false, null, null, null, GetCardSource());
			coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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