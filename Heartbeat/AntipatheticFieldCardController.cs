using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Heartbeat
{
    class AntipatheticFieldCardController : CardController
    {
        public AntipatheticFieldCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.Target == CharacterCard && !dd.DidDestroyTarget && dd.DamageType == DamageType.Radiant, (DealDamageAction dd) => ReduceNextDamageByDamageAmountResponse(dd), TriggerType.ReduceDamageLimited, TriggerTiming.After);
        }


        private IEnumerator ReduceNextDamageByDamageAmountResponse(DealDamageAction dd)
        {
            int dmg = dd.Amount;
            ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(dmg);
            rdse.NumberOfUses = 1;
            rdse.TargetCriteria.IsSpecificCard = CharacterCard;

            IEnumerator coroutine = GameController.AddStatusEffect(rdse, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Radiant, 1, false, 1, addStatusEffect: (DealDamageAction dd) => ReduceNextNonheroDamageResponse(dd), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = DealDamage(CharacterCard, CharacterCard, 1, DamageType.Radiant, addStatusEffect: (DealDamageAction dd) => ReduceNextNonheroDamageResponse(dd), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator ReduceNextNonheroDamageResponse(DealDamageAction dd)
        {
            if (dd.DidDealDamage && !IsHero(dd.Target))
            {
                ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                rdse.NumberOfUses = 1;
                rdse.SourceCriteria.IsSpecificCard = dd.Target;

                IEnumerator coroutine = AddStatusEffect(rdse, true);
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
                IEnumerator coroutine = DoNothing();
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
