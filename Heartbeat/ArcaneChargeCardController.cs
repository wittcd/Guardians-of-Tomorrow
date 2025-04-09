using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Heartbeat
{
    class ArcaneChargeCardController : CardController
    {
        public ArcaneChargeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            AddWhenDestroyedTrigger((DestroyCardAction dc) => IncreaseNextDamageResponse(), TriggerType.IncreaseDamage);
            AddTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && dd.DidDealDamage, (DealDamageAction dd) => OptionalSelfDestructResponse(), TriggerType.DestroyCard, TriggerTiming.After);
        }

        private IEnumerator OptionalSelfDestructResponse()
        {
            IEnumerator coroutine = GameController.DestroyCard(DecisionMaker, base.Card, true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator IncreaseNextDamageResponse()
        {
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(2);
            idse.NumberOfUses = 1;
            idse.SourceCriteria.IsSpecificCard = base.CharacterCard;
            IEnumerator coroutine = GameController.AddStatusEffect(idse, true, GetCardSource());
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
