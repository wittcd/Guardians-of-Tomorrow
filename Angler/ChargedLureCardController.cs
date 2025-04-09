using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Angler
{
    public class ChargedLureCardController : CardController
    {
        public ChargedLureCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddRedirectDamageTrigger((DealDamageAction dd) => dd.Target == CharacterCard || dd.Target.Identifier == "SSLangley", () => Card);
            AddWhenDestroyedTrigger((DestroyCardAction dc) => GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) || c.IsEnvironment, "ongoing or environment"), true, cardSource: GetCardSource()), TriggerType.DestroyCard);
        }
    }
}
