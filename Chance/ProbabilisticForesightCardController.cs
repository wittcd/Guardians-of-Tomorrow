using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class ProbabilisticForesightCardController : ChanceCardController
    {
        public ProbabilisticForesightCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => true, "turn taker"), SelectionType.RevealTopCardOfDeck, (TurnTaker tt) => RevealCards_MoveMatching_ReturnNonMatchingCards(DecisionMaker, tt.Deck, false, false, false, new LinqCardCriteria((Card c) => false), 1, 1, false, false, RevealedCardDisplay.ShowRevealedCards));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Function PlayTopCards = new Function(DecisionMaker, "Play the top card of each deck.", SelectionType.PlayTopCard, () => GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => true), SelectionType.PlayTopCard, (TurnTaker tt) => GameController.PlayTopCard(DecisionMaker, FindTurnTakerController(tt), cardSource: GetCardSource()), cardSource: GetCardSource()));
            Function DiscardTopCards = new Function(DecisionMaker, "Discard the top card of each deck.", SelectionType.DiscardFromDeck, () => GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => true), SelectionType.DiscardFromDeck, (TurnTaker tt) => GameController.DiscardTopCard(tt.Deck, null, cardSource: GetCardSource())));

            List<Function> functions = new List<Function>();
            functions.Add(PlayTopCards);
            functions.Add(DiscardTopCards);

            SelectFunctionDecision sfd = new SelectFunctionDecision(GameController, DecisionMaker, functions, true, null, null, null, GetCardSource());
            coroutine = GameController.SelectAndPerformFunction(sfd, null, null);
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
