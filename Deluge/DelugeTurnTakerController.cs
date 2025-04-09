using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Deluge
{
    class DelugeTurnTakerController : TurnTakerController
    {
        public DelugeTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            List<MoveCardDestination> locs = new List<MoveCardDestination>();
            locs.Add(new MoveCardDestination(base.CharacterCard.UnderLocation));
            IEnumerator coroutine = GameController.BulkMoveCards(this, GameController.FindEnvironmentTurnTakerController().TurnTaker.Deck.GetTopCards(H - 1), CharacterCard.UnderLocation, cardSource: CharacterCardController.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = PutCardsIntoPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("flood")), H - 2, true);
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
