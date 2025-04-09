using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Ludramor
{
    class EradicatorGunshipCardController : CardController
    {
        public EradicatorGunshipCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHighestHP(1, () => 2, new LinqCardCriteria((Card c) => !IsVillain(c), "non-villain"));
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DealDamageTo2Highest(), TriggerType.DealDamage);
        }

        private IEnumerator DealDamageTo2Highest()
        {
            List<Card> highest = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetsWithHighestHitPoints(1, 2, (Card c) => !IsVillain(c), highest, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.DealDamage(DecisionMaker, Card, (Card c) => highest.Contains(c), H - 1, DamageType.Projectile, cardSource: GetCardSource());
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
