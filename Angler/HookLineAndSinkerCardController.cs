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
    public class HookLineAndSinkerCardController : CardController
    {
        public HookLineAndSinkerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"After a one of your targets is dealt damage, {Angler} may deal the source of that damage 5 melee damage. If damage is dealt this way, destroy this card.."
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.DamageSource.IsTarget && dd.Target.Owner == TurnTaker, CounterAndSelfDestruct, TriggerType.DealDamage, TriggerTiming.After, isActionOptional: true);
        }

        private IEnumerator CounterAndSelfDestruct(DealDamageAction dd)
        {
            List<DealDamageAction> stored = new List<DealDamageAction>();

            IEnumerator coroutine = GameController.DealDamageToTarget(new DamageSource(GameController, CharacterCard), dd.DamageSource.Card, 5, DamageType.Melee, optional: true, storedResults: stored, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(stored))
            {
                coroutine = DestroyThisCardResponse(null);
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
