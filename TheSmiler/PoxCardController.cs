using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using Handelabra.Sentinels.Engine;

namespace GuardiansOfTomorrow.TheSmiler
{
    public class PoxCardController : CardController
    {
        public PoxCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, reveal the top card of each hero deck. This card deals the owner of each one-shot revealed this way 4 toxic damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt)), SelectionType.RevealTopCardOfDeck, RevealCardAndDoSomething, null, optional: false, null, null, allowAutoDecide: true, null, null, null, ignoreBattleZone: false, null, GetCardSource()), TriggerType.RevealCard);
            //When this card is destroyed, reveal cards from the villain deck until a Clue is revealed. Put it into play and shuffle the other revealed cards into the villain deck.
            AddWhenDestroyedTrigger((DestroyCardAction dc) => RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.IsClue, "clue"), 1), TriggerType.PutIntoPlay);
        }

        private IEnumerator RevealCardAndDoSomething(TurnTaker tt)
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.RevealCards(base.TurnTakerController, tt.Deck, 1, storedResults, fromBottom: false, RevealedCardDisplay.Message, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card revealedCard = GetRevealedCard(storedResults);
            if (revealedCard != null)
            {
                if (revealedCard.IsOneShot)
                {
                    //Deal hero 4 tox damage
                    List<Card> storedCharacter = new List<Card>();
                    coroutine = FindCharacterCardToTakeDamage(tt, storedCharacter, base.Card, 4, DamageType.Toxic);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    Card card = storedCharacter.FirstOrDefault();
                    if (card != null)
                    {
                        coroutine = DealDamage(base.Card, card, 4, DamageType.Toxic);
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
            List<Location> list = new List<Location>();
            list.Add(tt.Revealed);
            coroutine = base.GameController.CleanupCardsAtLocations(base.TurnTakerController, list, tt.Deck, toBottom: false, addInhibitorException: true, shuffleAfterwards: true, sendMessage: false, isDiscard: false, isReturnedToOriginalLocation: true, storedResults, GetCardSource());
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
