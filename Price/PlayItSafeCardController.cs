using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.Price
{
    class PlayItSafeCardController : CardController
    {
        public PlayItSafeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => RevealAndReplaceAndDamageIfPact(), TriggerType.RevealCard);
        }

        private IEnumerator RevealAndReplaceAndDamageIfPact()
        {
            //reveal the top card of your deck. You may replace or discard it. If you replaced a Pact card this way, {Price} deals up to 1 target 3 infernal damage
            IEnumerator coroutine = GameController.RevealCards(DecisionMaker, TurnTaker.Deck, (Card c) => true, 1, null, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<MoveCardAction> results = new List<MoveCardAction>();
            List<MoveCardDestination> locs = new List<MoveCardDestination>();
            locs.Add(new MoveCardDestination(TurnTaker.Deck));
            locs.Add(new MoveCardDestination(TurnTaker.Trash));
            coroutine = GameController.SelectCardFromLocationAndMoveIt(DecisionMaker, TurnTaker.Revealed, new LinqCardCriteria((Card c) => true, "card"), locs, isDiscardIfMovingtoTrash: true, storedResultsMove: results, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (results.FirstOrDefault().CardToMove.DoKeywordsContain("pact") && results.FirstOrDefault().Destination == TurnTaker.Deck)
            {
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 3, DamageType.Infernal, 1, false, 0, cardSource: GetCardSource());
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
