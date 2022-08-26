using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheHatter
{
    class QueenofHeartsCardController : CardController
    {
        public QueenofHeartsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroWithMostCards(false, null, null);
        }
        //"At the end of the villain turn, destroy {H-2} hero ongoing cards and this card deals the hero with the most cards in play 2 sonic damage."

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pc) => GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && c.IsOngoing, "hero ongoing"), H - 2, cardSource: GetCardSource()), TriggerType.DestroyCard);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pc) => DealDamageToMostCardsInPlay(base.Card, 1, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard), 2, DamageType.Sonic), TriggerType.DealDamage);
            base.AddTriggers();
        }
    }
}
