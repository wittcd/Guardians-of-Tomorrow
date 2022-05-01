using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace GuardiansOfTomorrow.Argus
{
    class HundredEyesCardController : CardController
    {
        public HundredEyesCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //search deck for camdrone, put into play, shuffle.
            IEnumerator coroutine = base.GameController.SelectCardFromLocationAndMoveIt(base.HeroTurnTakerController, base.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("camdrone"), () => "camdrone"), new MoveCardDestination[1]
            {
                new MoveCardDestination(base.TurnTaker.PlayArea)
            }, isPutIntoPlay: true, playIfMovingToPlayArea: true, shuffleAfterwards: true, optional: false, null, flipFaceDown: false, showOutput: false, null, isDiscardIfMovingtoTrash: false, allowAutoDecide: false, null, null, GetCardSource());
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
