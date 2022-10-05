using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class StaticCloudCardController : AlterationCardController
    {
        public StaticCloudCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
            rdse.UntilEndOfNextTurn(TurnTaker);
            rdse.SourceCriteria.IsHero = true;
            IEnumerator coroutine = AlterationPlayedResponse(DamageType.Lightning, AddStatusEffect(rdse));
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
