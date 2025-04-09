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
    public class TauntingBeaconCardController : CardController
    {
        public TauntingBeaconCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddRedirectDamageTrigger((DealDamageAction dd) => dd.Target == CharacterCard || dd.Target.Identifier == "SSLangley", () => Card);
            AddWhenDestroyedTrigger((DestroyCardAction dc) => DealDamage(Card, (Card c) => !IsHero(c), 2, DamageType.Sonic), TriggerType.DealDamage);
            //GameController.DealDamage(DecisionMaker, Card, (Card c) => !IsHeroTarget(Card), 2, DamageType.Psychic, cardSource: GetCardSource()), TriggerType.DealDamage
        }
    }
}