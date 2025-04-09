using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Heartbeat
{
    class EnergyLeakageCardController : CardController
    {
        public EnergyLeakageCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.Identifier == "ArcaneCharge", "copies of Arcane Charge", false, false, "Copy of Arcane Charge", "Copies of Arcane Charge", false));
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageBasedOnCharges, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageBasedOnCharges(PhaseChangeAction pc)
        {
            int charges = FindCardsWhere((Card c) => c.Identifier == "ArcaneCharge" && c.IsInPlayAndHasGameText).Count();

            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Radiant, charges, false, charges, cardSource: GetCardSource());
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
