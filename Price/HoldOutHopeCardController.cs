using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.Price
{
    class HoldOutHopeCardController : CardController
    {
        public HoldOutHopeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Radiant, 1, false, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.RevealAndReplaceCards(TurnTakerController, TurnTaker.Deck, 1, null, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            List<Card> played = new List<Card>();
            List<Function> f = new List<Function>();
            f.Add(new Function(DecisionMaker, "Play the top card of your deck", SelectionType.PlayTopCard, () => GameController.PlayTopCardOfLocation(TurnTakerController, TurnTaker.Deck, cardSource: GetCardSource(), playedCards: played)));
            f.Add(new Function(DecisionMaker, "Discard the top card of your deck", SelectionType.DiscardFromDeck, () => GameController.DiscardTopCard(TurnTaker.Deck, null, cardSource: GetCardSource())));
            SelectFunctionDecision opts = new SelectFunctionDecision(GameController, DecisionMaker, f, false, cardSource: GetCardSource());
            coroutine = GameController.SelectAndPerformFunction(opts);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            bool playedPact = false;
            foreach (Card c in played)
            {
                if (c.DoKeywordsContain("pact"))
                {
                    playedPact = true;
                }
            }

            if (playedPact)
            {
                coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => IsVillain(c), 2, DamageType.Infernal, cardSource: GetCardSource());
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
