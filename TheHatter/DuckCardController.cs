using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheHatter
{
    class DuckCardController : CardController
    {
        public DuckCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddCounterDamageTrigger((DealDamageAction dd) => dd.CardSource.Card.IsHero && dd.Target == base.Card, () => base.Card, () => base.Card, false, 2, DamageType.Psychic);
            base.AddTriggers();
        }
    }
}
