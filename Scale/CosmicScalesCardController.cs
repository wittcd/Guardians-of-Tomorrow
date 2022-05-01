using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class CosmicScalesCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public CosmicScalesCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, PickAThingResponse, TriggerType.Other, null, false);
		}

		private IEnumerator PickAThingResponse(PhaseChangeAction pc)
		{
			LinqCardCriteria isBalanceCard = new LinqCardCriteria((Card c) => c.DoKeywordsContain("balance", false, false), "balance");
			List<Function> list = new List<Function>();
			Function playBalance = new Function(DecisionMaker, "Put a balance card from hand into play", SelectionType.PutIntoPlay, () => SelectAndPlayCardFromHand(DecisionMaker, false, null, isBalanceCard, true, false, true, null));
			list.Add(playBalance);
			Function destroyBalance = new Function(DecisionMaker, "Destroy a balance card", SelectionType.DestroyCard, () => GameController.SelectAndDestroyCard(DecisionMaker, isBalanceCard, false, null, null, GetCardSource()), null, null, null);
			list.Add(destroyBalance);
			Function draw1 = new Function(DecisionMaker, "Draw a card", SelectionType.DrawCard, () => DrawCard(base.TurnTaker.ToHero(), false, null, true));
			list.Add(draw1);
			SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, DecisionMaker, list, optional: false, null, null, null, GetCardSource());
			IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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