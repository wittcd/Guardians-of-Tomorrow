using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheHatter
{
    class CaterpillarCardController : CardController
    {
        public CaterpillarCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        

        public override void AddTriggers()
        {
            AddReduceDamageTrigger((Card c) => c == base.Card, 1);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DestroyEnvAndMaybeDamageResponse, TriggerType.DestroyCard, null, false);
            base.AddTriggers();
        }

        private IEnumerator DestroyEnvAndMaybeDamageResponse(PhaseChangeAction pc)
        {
            List<DestroyCardAction> destroyed = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), false, destroyed, base.Card, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (destroyed.FirstOrDefault() != null && DidDestroyCard(destroyed.FirstOrDefault()))
            {
                if (!destroyed.FirstOrDefault().CardToDestroy.Card.IsTarget)
                {
                    coroutine = DealDamage(base.Card, (Card c) => IsHero(c), (Card c) => 2, DamageType.Projectile, false, false, null, null, null, false, null, null, false, false);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            } else
            {
                coroutine = DoNothing();
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
}
