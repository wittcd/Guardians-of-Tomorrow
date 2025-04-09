using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Legion
{
    class ConstructedPlasmaTurretCardController : CardController
    {
        public ConstructedPlasmaTurretCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP(2);
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pc) => DealDamageToLowestHP(Card, 2, (Card c) => IsHero(c), (Card c) => H - 1, DamageType.Energy), TriggerType.DealDamage);

        }
    }
}
