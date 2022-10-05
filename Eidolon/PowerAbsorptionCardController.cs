using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class PowerAbsorptionCardController : CardController
    {
        public PowerAbsorptionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger((DealDamageAction dd) => dd.DidDealDamage && dd.Target == base.CharacterCard, IncreaseDamageThisTurnOnlyResponse, TriggerType.IncreaseDamage, TriggerTiming.After);
        }

        private IEnumerator IncreaseDamageThisTurnOnlyResponse(DealDamageAction dd)
        {
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.SourceCriteria.IsSpecificCard = base.CharacterCard;
            idse.UntilThisTurnIsOver(Game);
            IEnumerator coroutine = AddStatusEffect(idse);
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
