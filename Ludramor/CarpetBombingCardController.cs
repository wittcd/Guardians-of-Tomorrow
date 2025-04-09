using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Ludramor
{
    class CarpetBombingCardController : CardController
    {
        public CarpetBombingCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(2, () => 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("spaceship"), "spaceship"));
            SpecialStringMaker.ShowListOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("employee"), "employee"));
        }

        public override IEnumerator Play()
        {
            List<Card> lowestShips = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetsWithLowestHitPoints(2, 1, (Card c) => c.DoKeywordsContain("spaceship"), lowestShips, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (lowestShips.FirstOrDefault() != null)
            {
                coroutine = GameController.DealDamage(DecisionMaker, lowestShips.FirstOrDefault(), (Card c) => !IsVillain(c), 3, DamageType.Fire, cardSource: GetCardSource());
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
