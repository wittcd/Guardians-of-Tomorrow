using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Deluge
{
    class SubmergedStreetsCardController : CardController
    {
        public SubmergedStreetsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            
        }

        public override void AddTriggers()
        {
            CannotDrawCards((TurnTakerController ttc) => ttc != null && ttc.IsHero && Journal.DrawCardEntriesThisTurn().Count() >= 1);
        }
    }
}
