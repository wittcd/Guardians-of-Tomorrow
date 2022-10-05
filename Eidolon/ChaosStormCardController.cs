using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class ChaosStormCardController : CardController
    {
		public ChaosStormCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == base.CharacterCard, 1);
            AddTrigger((DealDamageAction dd) => dd.DamageSource.Card.IsHeroCharacterCard, SelectDamageTypeForDealDamageAction, TriggerType.ChangeDamageType, TriggerTiming.Before);
        }

        
    }
}
