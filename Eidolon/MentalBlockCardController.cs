using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class MentalBlockCardController : AlterationCardController
    {
        public MentalBlockCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = AlterationPlayedResponse(DamageType.Psychic, AddBothStatusEffects());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator AddBothStatusEffects()
        {
            CannotUsePowersStatusEffect cupse = new CannotUsePowersStatusEffect();
            cupse.UntilStartOfNextTurn(base.TurnTaker);
            IEnumerator coroutine = AddStatusEffect(cupse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            PreventPhaseActionStatusEffect ppase = new PreventPhaseActionStatusEffect();
            ppase.ToTurnPhaseCriteria.Phase = Phase.UsePower;
            ppase.UntilStartOfNextTurn(base.TurnTaker);
            coroutine = AddStatusEffect(ppase);
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
