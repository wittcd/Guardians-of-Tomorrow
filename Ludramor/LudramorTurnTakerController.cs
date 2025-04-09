using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Ludramor
{
    class LudramorTurnTakerController : TurnTakerController
    {
        public LudramorTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            IEnumerator coroutine = PutCardsIntoPlay(new LinqCardCriteria((Card c) => c.Identifier == "PlanetcrackerBarge", "Planetcracker Barge", false), 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = PutCardsIntoPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("executive"), "executive"), base.H - 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = PutCardsIntoPlay(new LinqCardCriteria((Card c) => !c.DoKeywordsContain("executive") && c.IsTarget, "non-executive target"), base.H);
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
