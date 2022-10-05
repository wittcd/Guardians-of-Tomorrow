using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class UnbalancingScreechCardController : AlterationCardController
    {
        public UnbalancingScreechCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            CannotPlayCardsStatusEffect cpcse = new CannotPlayCardsStatusEffect();
            cpcse.UntilStartOfNextTurn(base.TurnTaker);
            cpcse.TurnTakerCriteria.IsHero = true;  
            IEnumerator coroutine = AlterationPlayedResponse(DamageType.Sonic, AddStatusEffect(cpcse));
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
