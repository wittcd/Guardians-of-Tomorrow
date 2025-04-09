using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Deluge
{
    class WaterloggedCardController : CardController
    {
        public WaterloggedCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => (IsEquipment(c) || IsOngoing(c)) && IsHero(c), "Hero ongoing or equipment"), false, cardSource: GetCardSource()), TriggerType.DestroyCard);
        }
    }
}
