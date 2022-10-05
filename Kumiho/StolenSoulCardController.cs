using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Kumiho
{
    class StolenSoulCardController : CardController
    {

        private bool TimeToEndGame;
		public StolenSoulCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
			base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
            TimeToEndGame = false;
		}

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //When this card enters play, put it in the play area of the hero with the lowest HP.
            List<Card> low = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsHeroCharacterCard, low, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            LinqCardCriteria lowestHero = new LinqCardCriteria((Card c) => low.Contains(c));
            coroutine = SelectCardThisCardWillMoveNextTo(lowestHero, storedResults, isPutIntoPlay, decisionSources);
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
            //Put the top 20 cards of that hero's deck face underneath this card.
            Card heroNextTo = GetCardThisCardIsNextTo();
            int cardsToMove = 10;
            if (IsGameChallenge)
            {
                cardsToMove = 20;
            }
            IEnumerator coroutine = GameController.BulkMoveCards(FindTurnTakerController(heroNextTo.Owner), heroNextTo.Owner.Deck.GetTopCards(cardsToMove), base.Card.UnderLocation, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

        }

        public override bool AskIfCardIsIndestructible(Card card)
		{
            //This card is indestructible so long as there are any cards under it.

            if (card == base.Card && FindCardsWhere((Card c) => c.Location == base.Card.UnderLocation).Count() > 0)
			{
				return card.BattleZone == base.CardWithoutReplacements.BattleZone;
			}
			return false;
		}

        public override void AddTriggers()
        {
            //AddTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && dd.DamageSource.Card == GetCardThisCardIsNextTo(), HighestHPRedirectResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
            AddTrigger((DealDamageAction dd) => dd.Target.IsHeroCharacterCard && dd.DidDealDamage && dd.DamageSource.Card == GetCardThisCardIsNextTo(), DiscardFromBeneathResponse, TriggerType.MoveCard, TriggerTiming.After);
            AddTrigger((GameAction ga) => GetCardThisCardIsNextTo() != null && GetCardThisCardIsNextTo().IsHeroCharacterCard && GetCardThisCardIsNextTo().IsIncapacitated && !TimeToEndGame, LoseWhenIncappedResponse, TriggerType.GameOver, TriggerTiming.After);
            base.AddTriggers();
        }

        private IEnumerator HighestHPRedirectResponse(DealDamageAction dd)
        {
            List<Card> stored = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetsWithHighestHitPoints(1, 1, (Card c) => c.IsHeroCharacterCard && c != GetCardThisCardIsNextTo(), stored, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = RedirectDamage(dd, TargetType.HighestHP, (Card c) => stored.Contains(c));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DiscardFromBeneathResponse(DealDamageAction dd)
        {
            Card topCard = base.Card.UnderLocation.TopCard;
            MoveCardDestination trashDestination = FindCardController(topCard).GetTrashDestination();
            IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, topCard, trashDestination.Location, trashDestination.ToBottom, isPutIntoPlay: false, playCardIfMovingToPlayArea: true, null, showMessage: true, null, null, null, evenIfIndestructible: false, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (FindCardsWhere((Card c) => c.Location == base.Card.UnderLocation).Count() <= 0)
            {
                coroutine = base.GameController.SendMessageAction(base.Card.Title + " is removed from the game.", Priority.Medium, GetCardSource(), null, showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.MoveCard(FindTurnTakerController(base.TurnTaker), base.Card, base.TurnTaker.OutOfGame, cardSource: GetCardSource());
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

        private IEnumerator LoseWhenIncappedResponse(GameAction ga)
        {
            TimeToEndGame = true;
            string defeatString = "Kumiho has devoured " + GetCardThisCardIsNextTo().Title + "'s soul!";
            IEnumerator coroutine = base.GameController.SendMessageAction(defeatString, Priority.Critical, GetCardSource(), null, showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            IEnumerator coroutine2 = base.GameController.GameOver(EndingResult.AlternateDefeat, defeatString, showEndingTextAsMessage: false, null, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
        }
    }
}
