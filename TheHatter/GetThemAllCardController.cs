using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheHatter
{
    class GetThemAllCardController : CardController
    {
        public GetThemAllCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        { }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            if (FindCardsWhere(new LinqCardCriteria((Card c) => c.DoKeywordsContain("thrall"), "thrall")).Count() > 0)
            {
                coroutine = DealDamage((Card c) => c.DoKeywordsContain("thrall") && c.IsInPlayAndHasGameText, (Card c) => c.IsHero, (Card c) => 1, DamageType.Melee);
            }
            else
            {
                coroutine = PlayTheTopCardOfTheVillainDeckResponse(null);
            }
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
