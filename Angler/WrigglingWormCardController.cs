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
    public class WrigglingWormCardController : CardController
    {
        public WrigglingWormCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddRedirectDamageTrigger((DealDamageAction dd) => dd.Target == CharacterCard || dd.Target.Identifier == "SSLangley", () => Card);
            AddWhenDestroyedTrigger((DestroyCardAction dc) => GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), 3, DamageType.Melee, 1, false, 0, cardSource:GetCardSource()), TriggerType.DealDamage);
        }
    }
}
