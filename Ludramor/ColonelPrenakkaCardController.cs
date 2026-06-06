using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Ludramor
{
    class ColonelPrenakkaCardController : CardController
    {
        public ColonelPrenakkaCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => GameController.DoesCardContainKeyword(c, "soldier"), "soldier"));
            SpecialStringMaker.ShowHighestHP(1, () => 1, new LinqCardCriteria((Card c) => !IsVillain(c)));
        }

        private IEnumerator RedirectToSoldier(DealDamageAction dd)
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card card) => card != Card && GameController.DoesCardContainKeyword(card, "soldier"), storedResults, dd, null, evenIfCannotDealDamage: false, optional: false, null, ignoreBattleZone: false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card2 = storedResults.FirstOrDefault();
            if (card2 != null)
            {
                IEnumerator coroutine2 = base.GameController.RedirectDamage(dd, card2, isOptional: false, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
        }

        private int numSoldier()
        {
            return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && GameController.DoesCardContainKeyword(c, "soldier")).Count();
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger((DealDamageAction dd) => GameController.DoesCardContainKeyword(dd.DamageSource.Card, "soldier"), (DealDamageAction dd) => numSoldier());
            AddTrigger((DealDamageAction dd) => dd.Target == Card, RedirectToSoldier, TriggerType.RedirectDamage, TriggerTiming.Before);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DealDamageToHighestHP(Card, 1, (Card c) => !IsVillain(c), (Card c) => H - 1, DamageType.Projectile), TriggerType.DealDamage);
        }
    }
}
