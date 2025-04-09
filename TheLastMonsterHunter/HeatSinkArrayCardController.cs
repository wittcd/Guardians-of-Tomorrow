using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class HeatSinkArrayCardController : CardController
    {
        public HeatSinkArrayCardController(Card card, TurnTakerController turnTakerController)
                    : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageType == DamageType.Fire && IsVillainTarget(dd.Target));
        }
    }
}
