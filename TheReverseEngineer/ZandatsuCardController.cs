using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using GuardiansOfTomorrow.TheReverseEngineer;

namespace GuardiansOfTomorrow.TheReverseEngineer
{
    class ZandatsuCardController : TheReverseEngineerCardController
    {
        public ZandatsuCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator FromTrashAction()
        {
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Melee, 1, false, 0, true, additionalCriteria: (Card c) => c.HitPoints <= 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            AddTrigger((DestroyCardAction dc) => dc.CardSource != null && dc.CardSource.Card.Owner == TurnTaker && dc.CardToDestroy.Card.IsTarget && dc.WasCardDestroyed && (dc.DealDamageAction == null || dc.DealDamageAction.DamageSource.Card == base.CharacterCard) && (dc.DealDamageAction != null || dc.CardSource.Card.IsOneShot || dc.CardSource.Card.HasPowers || dc.ResponsibleCard == CharacterCard), (DestroyCardAction dc) => GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource()), TriggerType.GainHP, TriggerTiming.After);
        }
    }
}
