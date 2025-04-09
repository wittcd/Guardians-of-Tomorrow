using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Legion
{
    class NaniteTrapCardController : CardController
    {
        public NaniteTrapCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger((DealDamageAction dd) => dd.DidDealDamage && dd.Target == base.Card, (DealDamageAction dd) => DealDamageAndInhibitResponse(dd), TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageAndInhibitResponse(DealDamageAction dd)
        {
            List<DealDamageAction> stored = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamage(Card, dd.DamageSource.Card, 2, DamageType.Melee, storedResults: stored, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(stored))
            {
                CannotDealDamageStatusEffect cddse = new CannotDealDamageStatusEffect();
                cddse.SourceCriteria.IsSpecificCard = stored.FirstOrDefault().Target;
                cddse.UntilStartOfNextTurn(TurnTaker);
                coroutine = AddStatusEffect(cddse);
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
