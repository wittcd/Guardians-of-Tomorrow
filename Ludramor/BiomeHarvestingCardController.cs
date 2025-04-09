using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Ludramor
{
    class BiomeHarvestingCardController : CardController
    {
        public BiomeHarvestingCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(1, () => 1, new LinqCardCriteria((Card c) => c.IsEnvironmentTarget, "environment target", false));
        }

        public override IEnumerator Play()
        {
            List<Card> lowests = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetsWithLowestHitPoints(1, 1, (Card c) => c.IsEnvironment, lowests, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card l = lowests.FirstOrDefault();
            if (l != null)
            {
                List<DestroyCardAction> destroys = new List<DestroyCardAction>();
                coroutine = GameController.DestroyCard(DecisionMaker, l, storedResults: destroys, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (DidDestroyCard(destroys))
                {
                    coroutine = GameController.GainHP(DecisionMaker, (Card c) => c.DoKeywordsContain("executive"), H + 2, cardSource: GetCardSource());
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
                    coroutine = LowestDealsDamageToAllNonVillain();
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
            else
            {
                coroutine = LowestDealsDamageToAllNonVillain();
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

        IEnumerator LowestDealsDamageToAllNonVillain()
        {
            List<Card> lowestvillains = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetsWithLowestHitPoints(1, 1, (Card c) => IsVillainTarget(c), lowestvillains, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card lowv = lowestvillains.FirstOrDefault();
            if (lowv != null)
            {
                coroutine = GameController.DealDamage(DecisionMaker, lowv, (Card c) => !c.IsVillain, 2, DamageType.Projectile, cardSource: GetCardSource());
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
