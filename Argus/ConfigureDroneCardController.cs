using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace GuardiansOfTomorrow.Argus
{
    class ConfigureDroneCardController : CardController
    {
        public ConfigureDroneCardController(Card card, TurnTakerController turnTakerController)
          : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
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

            //you may use a power and activate an operate text
            coroutine = GameController.SelectAndUsePower(DecisionMaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.SelectAndActivateAbility(DecisionMaker, "operate", new LinqCardCriteria((Card c) => c.IsInPlay, "camdrone"), null, true, GetCardSource());
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
