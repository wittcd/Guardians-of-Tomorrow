using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace GuardiansOfTomorrow.Argus
{
    class SemiAutonomousProtocolsCardController : CardController
    {
        public SemiAutonomousProtocolsCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pc) => ActivateAnOperate(), TriggerType.Other);
        }

        private IEnumerator ActivateAnOperate() 
        {
            LinqCardCriteria isCamdrone = new LinqCardCriteria((Card c) => c.DoKeywordsContain("camdrone") && c.IsInPlay, "camdrone");
            IEnumerator coroutine = GameController.SelectAndActivateAbility(DecisionMaker, "operate", isCamdrone, null, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
