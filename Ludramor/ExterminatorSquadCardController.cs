using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.Ludramor
{
    class ExterminatorSquadCardController : CardController
    {
        public ExterminatorSquadCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => GameController.DealDamage(DecisionMaker, Card, (Card c) => !IsVillain(c), 1, DamageType.Projectile, cardSource: GetCardSource()), TriggerType.DealDamage);
        }
    }
}
