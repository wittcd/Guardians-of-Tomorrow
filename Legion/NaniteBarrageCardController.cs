using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Legion
{
    class NaniteBarrageCardController : CardController
    {
        public NaniteBarrageCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(2);
        }

        public override IEnumerator Play()
        {
            List<Card> highest = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithHighestHitPoints(2, (Card c) => IsHero(c), highest, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidFindCard(highest))
            {
                coroutine = DealDamage((Card c) => c == CharacterCard, (Card c) => c == highest.FirstOrDefault(), (Card c) => FindCardsWhere((Card c2) => c2.IsDevice && c2.IsInPlay).Count() + 2, DamageType.Projectile);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                coroutine = DoNothing();
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
