using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.Price
{
    class OnTheRunCardController : CardController
    {
        public OnTheRunCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddReduceDamageTrigger((Card c) => c == CharacterCard, 1);
            AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay.DoKeywordsContain("pact"), (CardEntersPlayAction cep) => PickAndShuffleResponse(), TriggerType.MoveCard, TriggerTiming.After);
        }

        private IEnumerator PickAndShuffleResponse()
        {
            List<MoveCardDestination> m = new List<MoveCardDestination>();
            m.Add(new MoveCardDestination(TurnTaker.Deck));
            IEnumerator coroutine = GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, TurnTaker.Trash, 0, 3, new LinqCardCriteria((Card c) => true, "any"), m, shuffleAfterwards: false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.ShuffleLocation(TurnTaker.Deck, cardSource: GetCardSource());
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
