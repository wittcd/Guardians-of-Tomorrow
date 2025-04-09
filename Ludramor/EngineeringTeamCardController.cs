using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.Ludramor
{
    class EngineeringTeamCardController : CardController
    {
        public EngineeringTeamCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, delegate
            {
                GameController gameController = base.GameController;
                HeroTurnTakerController decisionMaker = DecisionMaker;
                Func<Card, bool> criteria = (Card card) => card.DoKeywordsContain("spaceship") && IsVillain(card);
                CardSource cardSource = GetCardSource();
                return gameController.GainHP(decisionMaker, criteria, 1, null, optional: false, null, null, null, cardSource);
            }, TriggerType.GainHP);
        }
    }
}
