using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Ludramor
{
    class AdvanceScoutCardController : CardController
    {
        public AdvanceScoutCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            
        }

        public override void AddTriggers()
        {
            AddImmuneToDamageTrigger((DealDamageAction dd) => (dd.Target.DoKeywordsContain("spaceship") || dd.Target.DoKeywordsContain("executive")) && (dd.DamageSource.Card != null && dd.DamageSource.Card.IsEnvironment));
        }
    }
}
