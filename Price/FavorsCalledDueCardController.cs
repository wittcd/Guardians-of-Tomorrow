using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Price
{
	class FavorsCalledDueCardController : CardController
	{
		public FavorsCalledDueCardController(Card card, TurnTakerController turnTakerController)
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

		public override IEnumerator Play()
		{
			List<DiscardCardAction> store = new List<DiscardCardAction>();
			IEnumerator coroutine = GameController.SelectAndDiscardCards(DecisionMaker, 2, true, 2, store, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if (!DidDiscardCards(store, 2))
			{
				coroutine = DealDamage(CharacterCard, CharacterCard, 3, DamageType.Infernal, isIrreducible: true, cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
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
