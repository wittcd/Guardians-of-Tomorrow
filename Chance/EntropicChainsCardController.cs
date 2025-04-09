using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class EntropicChainsCardController : ChanceCardController
    {
        public EntropicChainsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), 3, DamageType.Projectile, 1, false, 1, addStatusEffect: PreventNextDamageResponse, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator PreventNextDamageResponse(DealDamageAction dd)
        {
            CannotDealDamageStatusEffect cddse = new CannotDealDamageStatusEffect();
            cddse.NumberOfUses = 1;
            cddse.SourceCriteria.IsSpecificCard = dd.Target;
            IEnumerator coroutine = AddStatusEffect(cddse);
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
