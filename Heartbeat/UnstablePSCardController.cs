using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Heartbeat
{
    class UnstablePSCardController : CardController
    {
        public UnstablePSCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.Identifier == "ArcaneCharge", "copies of Arcane Charge", false, false, "Copy of Arcane Charge", "Copies of Arcane Charge", false));
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == base.CharacterCard && dd.DamageType == DamageType.Radiant, 1);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DealDamageBasedOnCharges(), TriggerType.DealDamage);
        }

        private IEnumerator DealDamageBasedOnCharges()
        {
            int charges = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "ArcaneCharge").Count();

            IEnumerator coroutine = DealDamage(base.CharacterCard, base.CharacterCard, charges, DamageType.Radiant, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine = DestroyThisCardResponse(null);
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
