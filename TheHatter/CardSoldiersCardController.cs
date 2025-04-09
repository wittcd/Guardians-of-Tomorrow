using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheHatter
{
    class CardSoldiersCardController : CardController
    {
        public CardSoldiersCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP(1, 3, false);
        }
        //"At the end of the villain turn, destroy {H-2} hero ongoing cards and this card deals the hero with the most cards in play 2 sonic damage."

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pc) => DealDamageToHighestHP(base.Card, 1, (Card c) => IsHero(c), (Card c) => 1, DamageType.Melee, true, false, null, () => 3, null, null, false, false), TriggerType.DealDamage);
            base.AddTriggers();
        }
    }
}
