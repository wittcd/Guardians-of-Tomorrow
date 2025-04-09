using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Arsenal
{
    public class ReadyShotCardController : CardController
    {
        public ReadyShotCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cep) => !IsHero(cep.CardEnteringPlay) && cep.CardEnteringPlay.IsTarget, PreemptiveDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator PreemptiveDamageResponse(CardEntersPlayAction cep)
        {
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            Card target = cep.CardEnteringPlay;
            IEnumerator coroutine = DealDamage(CharacterCard, target, 2, DamageType.Projectile, false, true, storedResults: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(storedResults))
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
