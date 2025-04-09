using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheSmiler
{
    public class PecksCardController : CardController
    {
        public PecksCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, play the top card of the villain deck, then destroy 1 Clue or hero non-character card in the hero play area with the most non-character cards.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => EndOfTurnResponse(), new TriggerType[] { TriggerType.PlayCard, TriggerType.DestroyCard });
            //When this card is destroyed, reveal cards from the villain deck until a Clue is revealed. Put it into play and shuffle the other revealed cards into the villain deck.
            AddWhenDestroyedTrigger((DestroyCardAction dc) => RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.IsClue, "clue"), 1), TriggerType.PutIntoPlay);
        }

        private IEnumerator EndOfTurnResponse()
        {
            IEnumerator coroutine = GameController.PlayTopCard(DecisionMaker, TurnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<TurnTaker> storedResults = new List<TurnTaker>();
            coroutine = FindHeroWithMostCardsInPlayArea(storedResults, 1, 1, null, new LinqCardCriteria((Card c) => !c.IsCharacter, "non-character"));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => (c.IsClue || (IsHero(c) && !c.IsCharacter) && c.Location == storedResults.FirstOrDefault().PlayArea), "clue or hero non-character"), false, cardSource: GetCardSource());
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
