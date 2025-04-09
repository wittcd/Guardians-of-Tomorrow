using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheHatter
{
    class DeadlyTieCardController : CardController
    {
        public DeadlyTieCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(2, 1, false);
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => DealDamageToHighestHP(base.CharacterCard, 2, (Card c) => IsHero(c), (Card c) => H, DamageType.Lightning), TriggerType.DealDamage);
            base.AddTriggers();
        }
    }
}
