using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class PulsejetCardController : CardController
    {
        public PulsejetCardController(Card card, TurnTakerController turnTakerController)
                    : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.DamageType == DamageType.Melee, (DealDamageAction dd) => 1);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DealDamageToHighestHP(CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => H - 2, DamageType.Melee), TriggerType.DealDamage);
        }
    }
}
