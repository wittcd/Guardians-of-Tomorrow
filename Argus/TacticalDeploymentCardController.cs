using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace GuardiansOfTomorrow.Argus
{
    class TacticalDeploymentCardController : CardController
    {
        public TacticalDeploymentCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay.DoKeywordsContain("camdrone"), (CardEntersPlayAction cep) => GameController.SelectAndActivateAbility(DecisionMaker, "operate", new LinqCardCriteria((Card c) => c == cep.CardEnteringPlay, "card entering play"), null, true, GetCardSource()), TriggerType.Other, TriggerTiming.After);
        }
    }
}
