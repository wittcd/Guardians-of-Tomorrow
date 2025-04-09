using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{
    class FlyingBoulderCardController : CardController
    {
        public FlyingBoulderCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DealDamageAndTakeDamage(), TriggerType.DealDamage);
        }

        private IEnumerator DealDamageAndTakeDamage()
        {
            List<DealDamageAction> ddlist = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), 3, DamageType.Melee, 3, false, 0, storedResultsDamage: ddlist, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<Card> targets = new List<Card>();
            foreach (DealDamageAction dd in ddlist)
            {
                if (!targets.Contains(dd.Target) && dd.DidDealDamage && !dd.DidDestroyTarget)
                {
                    coroutine = DealDamage(dd.Target, Card, 2, DamageType.Melee, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    targets.Add(dd.Target);
                }
            }
        }
    }
}
