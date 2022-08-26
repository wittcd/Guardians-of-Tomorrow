using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheHatter
{
    class BoomingSalesCardController : CardController
    {
        public BoomingSalesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        //At the start of the villain turn, reveal cards from the villain deck until a Thrall is revealed. Put the revealed card into play and discard the remaining cards.

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, FindThrallResponse, TriggerType.PutIntoPlay, null, false);
            base.AddTriggers();
        }

        private IEnumerator FindThrallResponse(PhaseChangeAction pc)
        {
            IEnumerator coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, playMatchingCards: false, putMatchingCardsIntoPlay: true, moveMatchingCardsToHand: false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("thrall"), "thrall"), 1);
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
