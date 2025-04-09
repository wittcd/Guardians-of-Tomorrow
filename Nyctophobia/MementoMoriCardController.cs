using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Nyctophobia
{
    internal class MementoMoriCardController : CardController
    {
        public MementoMoriCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger((DealDamageAction dd) => !IsHero(dd.Target) && dd.Target.HitPoints <= 5 && dd.DamageSource.Card == CharacterCard, 1);
            AddTrigger((DestroyCardAction dc) => dc.CardSource != null && dc.CardSource.Card.Owner == TurnTaker && dc.CardToDestroy.Card.IsTarget && dc.WasCardDestroyed && (dc.DealDamageAction == null || dc.DealDamageAction.DamageSource.Card == base.CharacterCard) && (dc.DealDamageAction != null || dc.CardSource.Card.IsOneShot || dc.CardSource.Card.HasPowers || dc.ResponsibleCard == CharacterCard), (DestroyCardAction dc) => GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Psychic, 1, false, 0, cardSource: GetCardSource()), TriggerType.DealDamage, TriggerTiming.After);
        }
    }
}
