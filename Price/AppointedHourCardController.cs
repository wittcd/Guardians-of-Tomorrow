using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Price
{
    class AppointedHourCardController : CardController
    {
		public AppointedHourCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
			AddInhibitorException((GameAction ga) => ga is PlayCardAction && base.Card.Location.IsHand);
		}

		private IEnumerator PlayFromHandResponse()
		{
			IEnumerator coroutine = base.GameController.SendMessageAction(base.Card.Title + " puts itself into play.", Priority.High, GetCardSource(), null, showCardSource: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			GameController gameController = base.GameController;
			TurnTakerController turnTakerController = base.TurnTakerController;
			Card card = base.Card;
			CardSource cardSource = GetCardSource();
			coroutine = gameController.PlayCard(turnTakerController, card, isPutIntoPlay: true, null, optional: false, null, null, evenIfAlreadyInPlay: false, null, null, null, associateCardSource: false, fromBottom: false, canBeCancelled: true, cardSource);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}
		public override void AddStartOfGameTriggers()
		{
			AddTrigger((DrawCardAction d) => d.DrawnCard == base.Card, (DrawCardAction d) => PlayFromHandResponse(), new TriggerType[2]
			{
				TriggerType.PutIntoPlay,
				TriggerType.Hidden
			}, TriggerTiming.After, null, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: true);
			AddTrigger((MoveCardAction m) => m.Destination == base.HeroTurnTaker.Hand && m.CardToMove == base.Card, (MoveCardAction m) => PlayFromHandResponse(), new TriggerType[2]
			{
				TriggerType.PutIntoPlay,
				TriggerType.Hidden
			}, TriggerTiming.After, null, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: true);
		}
		public override IEnumerator PerformEnteringGameResponse()
		{
			IEnumerator coroutine = ((!base.Card.IsInHand) ? base.PerformEnteringGameResponse() : PlayFromHandResponse());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public override IEnumerator Play()
        {
			CannotPlayCardsStatusEffect cpse = new CannotPlayCardsStatusEffect();
			cpse.TurnTakerCriteria.IsSpecificTurnTaker = TurnTaker;
			cpse.UntilEndOfNextTurn(TurnTaker);
			CannotUsePowersStatusEffect cupse = new CannotUsePowersStatusEffect();
			cupse.TurnTakerCriteria.IsSpecificTurnTaker = TurnTaker;
			cupse.UntilEndOfNextTurn(TurnTaker);
			List<Function> funcs = new List<Function>();
			funcs.Add(new Function(DecisionMaker, "Cannot play cards", SelectionType.CannotPlayCards, () => GameController.AddStatusEffect(cpse, true, GetCardSource())));
			funcs.Add(new Function(DecisionMaker, "Cannot use powers", SelectionType.Custom, () => GameController.AddStatusEffect(cupse, true, GetCardSource())));
			funcs.Add(new Function(DecisionMaker, "Deal self 3 infernal damage.", SelectionType.DealDamage, () => DealDamage(CharacterCard, CharacterCard, 3, DamageType.Infernal, isIrreducible: true, cardSource: GetCardSource())));
			SelectFunctionDecision sfd = new SelectFunctionDecision(GameController, DecisionMaker, funcs, false, cardSource: GetCardSource());

			IEnumerator coroutine = GameController.SelectAndPerformFunction(sfd);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			coroutine = GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource());
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
