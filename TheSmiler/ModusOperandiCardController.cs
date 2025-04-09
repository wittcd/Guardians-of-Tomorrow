using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheSmiler
{
    public class ModusOperandiCardController : CardController
    {
        public ModusOperandiCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            List<SelectTurnTakerDecision> turntakers = new List<SelectTurnTakerDecision>();
            GameController gameController = base.GameController;
            HeroTurnTakerController decisionMaker = DecisionMaker;
            Func<TurnTaker, bool> additionalCriteria = (TurnTaker tt) => IsHero(tt);
            CardSource cardSource = GetCardSource();
            IEnumerator coroutine = gameController.SelectTurnTaker(decisionMaker, SelectionType.MoveCardToPlayArea, turntakers, optional: false, allowAutoDecide: false, additionalCriteria, null, null, checkExtraTurnTakersInstead: false, canBeCancelled: true, ignoreBattleZone: false, cardSource);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            SelectTurnTakerDecision selectTurnTakerDecision = turntakers.FirstOrDefault();
            if (selectTurnTakerDecision != null && selectTurnTakerDecision.SelectedTurnTaker != null && turntakers != null)
            {
                storedResults.Add(new MoveCardDestination(selectTurnTakerDecision.SelectedTurnTaker.PlayArea, toBottom: false, showMessage: true));
                yield break;
            }
        }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => RevealTopCardAndMaybeDiscard(), new TriggerType[] { TriggerType.RevealCard, TriggerType.DiscardCard, TriggerType.ShuffleTrashIntoDeck });
        }

        private IEnumerator RevealTopCardAndMaybeDiscard()
        {
            //At the start of the villain turn, reveal and replace the top card of the villain deck,
            IEnumerator coroutine = GameController.RevealAndReplaceCards(TurnTakerController, TurnTaker.Deck, 1, null, null, false, RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //then the hero whose play area this is in may discard 2 cards.
            List<DiscardCardAction> discards = new List<DiscardCardAction>();
            TurnTaker currentHero = Card.Location.OwnerTurnTaker;
            coroutine = GameController.SelectAndDiscardCards(FindHeroTurnTakerController(currentHero.ToHero()), 2, true, 0, discards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If 2 cards are discarded this way, shuffle the villain trash into the villain deck.
            if (DidDiscardCards(discards, 2))
            {
                coroutine = GameController.ShuffleTrashIntoDeck(TurnTakerController, cardSource: GetCardSource());
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
