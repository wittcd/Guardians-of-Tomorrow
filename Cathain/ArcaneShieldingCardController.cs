using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace GuardiansOfTomorrow.Cathain
{
    public class ArcaneShieldingCardController : CardController
    {
        public ArcaneShieldingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            //Redirect damage dealt to {Cathain} to one of your Loci.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == CharacterCard, (DealDamageAction dd) => GameController.SelectTargetAndRedirectDamage(DecisionMaker, (Card c) => c.Owner == TurnTaker && GameController.DoesCardContainKeyword(c, "locus"), dd, cardSource: GetCardSource()), TriggerType.RedirectDamage, TriggerTiming.Before);
            //At the start of turn, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DestroyThisCardResponse(null), TriggerType.DestroySelf);
        }
    }
}
