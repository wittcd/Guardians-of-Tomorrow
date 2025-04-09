using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class PointDefenseCannonsCardController : CardController
    {
        public PointDefenseCannonsCardController(Card card, TurnTakerController turnTakerController)
                    : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cep) => IsHero(cep.CardEnteringPlay), (CardEntersPlayAction cep) => DealDamage(CharacterCard, cep.CardEnteringPlay.Owner.CharacterCard, 1, DamageType.Projectile, cardSource: GetCardSource()), TriggerType.DealDamage, TriggerTiming.After);
        }
    }
}
