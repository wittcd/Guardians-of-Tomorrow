using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class BlazingAuraCardController : AlterationCardController
    {
        public BlazingAuraCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.UntilEndOfNextTurn(TurnTaker);
            idse.SourceCriteria.IsSpecificCard = base.CharacterCard;
            IEnumerator coroutine = AlterationPlayedResponse(DamageType.Fire, AddStatusEffect(idse));
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
