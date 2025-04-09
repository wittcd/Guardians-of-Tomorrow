using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheHatter
{
    class ExecutionerCardController : CardController
    {
        public ExecutionerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => DealDamageToLowestHP(base.Card, 1, (Card c) => !IsVillain(c) || (c.DoKeywordsContain("thrall") && !(c == base.Card)), (Card c) => H, DamageType.Melee), TriggerType.DealDamage);
            base.AddTriggers();
        }
    }
}
