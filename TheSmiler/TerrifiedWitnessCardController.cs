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
    public class TerrifiedWitnessCardController : CardController
    {
        public TerrifiedWitnessCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //When this card is destroyed, it deals each hero target 2 psychic damage.
            AddWhenDestroyedTrigger((DestroyCardAction dc) => GameController.DealDamage(DecisionMaker, Card, (Card c) => IsHeroTarget(c), 3, DamageType.Psychic, true, cardSource: GetCardSource()), TriggerType.DealDamage);
            //At the end of the villain turn, {TheSmiler} deals this card 3 irreducible psychic damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c == Card, 3, DamageType.Psychic, true, cardSource: GetCardSource()), TriggerType.DealDamage);
            //At the start of the villain turn, you may destroy up to 2 hero non-character cards. If 2 cards are destroyed this way, reveal cards from the villain deck until a Clue is revealed and put it into play, then shuffle the remaining cards and this card into the villain deck.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => StartOfTurnResponse(), new TriggerType[] { TriggerType.DestroyCard, TriggerType.RevealCard, TriggerType.PlayCard, TriggerType.MoveCard });
        }

        private IEnumerator StartOfTurnResponse()
        {
            List<DestroyCardAction> destroys = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => !c.IsCharacter && IsHero(c), "hero non-character"), 2, false, 0, storedResultsAction: destroys, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDestroyCards(destroys, 2))
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

                coroutine = GameController.MoveCard(TurnTakerController, Card, TurnTaker.Trash, cardSource: GetCardSource());
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
