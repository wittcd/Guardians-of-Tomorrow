using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class TheLastMonsterHunterTurnTakerController : TurnTakerController
    {
        public TheLastMonsterHunterTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            // Cards are revealed from the top of the villain deck until 2 Weapons and 1 Upgrade are revealed and put into play.
            // Other revealed cards are shuffled back into the villain deck.

            IEnumerator coroutine = PutCardsIntoPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("weapon"), "weapon"), 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = PutCardsIntoPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("upgrade"), "upgrade"), 1);
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
