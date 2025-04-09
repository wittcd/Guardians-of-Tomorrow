using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheSmiler
{
    public class LabyrinthOfCluesCardController : CardController
    {
        public LabyrinthOfCluesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //{TheSmiler} deals each hero 2 psychic damage.
            IEnumerator coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => IsHeroCharacterCard(c), 2, DamageType.Psychic, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Each hero may discard any number of cards.
            List<DiscardCardAction> stored = new List<DiscardCardAction>();
            coroutine = GameController.EachPlayerDiscardsCards(0, null, stored, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If more than {H} cards are discarded this way, reveal cards from the villain deck until a Secret is revealed and put it into play. Shuffle the other cards into the villain deck.
            if (DidDiscardCards(stored, H + 1, true))
            {
                coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.IsClue, "clue"), 1);
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
