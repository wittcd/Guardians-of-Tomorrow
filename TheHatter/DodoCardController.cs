using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheHatter
{
    class DodoCardController : CardController
    {
        public DodoCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroWithMostCards(false, null, null);
        }
        //At the end of the villain turn, each Thrall regains 1 HP and this card deals the hero target with the second lowest HP 2 sonic damage.

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pc) => GameController.GainHP(DecisionMaker, (Card c) => c.DoKeywordsContain("thrall"), 1, null, false, null, null, null, GetCardSource()), TriggerType.GainHP, null, false);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pc) => DealDamageToLowestHP(base.Card, 2, (Card c) => IsHero(c), (Card c) => 2, DamageType.Sonic), TriggerType.DealDamage);
            base.AddTriggers();
        }
    }
}
