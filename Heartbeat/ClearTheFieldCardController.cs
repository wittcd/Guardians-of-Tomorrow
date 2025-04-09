using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Heartbeat
{
    class ClearTheFieldCardController : CardController
    {
        public ClearTheFieldCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => IsOngoing(c), "ongoing", true, false));
        }

        public override IEnumerator Play()
        {
            List<DestroyCardAction> stored = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) && c.IsInPlay), 3, false, 0, storedResultsAction: stored, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDestroyCard(stored))
            {
                int destroycount = 0;
                foreach (DestroyCardAction dc in stored)
                {
                    if (DidDestroyCard(dc) && IsHero(dc.CardToDestroy.Card))
                    {
                        destroycount++;
                    }
                }
                coroutine = GameController.DrawCards(TurnTakerController.ToHero(), destroycount, upTo: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Radiant, destroycount, false, 0, cardSource: GetCardSource());
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
