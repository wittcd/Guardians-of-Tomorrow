using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class FoxEchoCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public FoxEchoCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//end of turn card draw
			AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, EndOfTurnResponse, TriggerType.DrawCard);
			//when destroyed, may play an ongoing
			AddWhenDestroyedTrigger(OnDestroyResponse, TriggerType.PlayCard);
		}

		private IEnumerator OnDestroyResponse(DestroyCardAction dc)
		{
			IEnumerator coroutine = SelectAndPlayCardsFromHand(DecisionMaker, 1, optional: true, cardCriteria: new LinqCardCriteria((Card c) => IsOngoing(c), "ongoing"), requiredDecisions: 0);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public IEnumerator EndOfTurnResponse(PhaseChangeAction p)
		{
			List<DrawCardAction> storedResults = new List<DrawCardAction>();
			IEnumerator coroutine = EachPlayerDrawsACard(null, optional: true, allowAutoDraw: true, storedResults, allowAutoDrawOrder: false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (DidDrawCards(storedResults, H) || DidDrawCards(storedResults, H - 1))
			{
				GameController gameController = base.GameController;
				HeroTurnTakerController decisionMaker = DecisionMaker;
				Card card = base.Card;
				CardSource cardSource = GetCardSource();
				coroutine = base.GameController.DestroyCard(DecisionMaker, base.Card, optional: false, null, null, null, null, null, null, null, null, cardSource);
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