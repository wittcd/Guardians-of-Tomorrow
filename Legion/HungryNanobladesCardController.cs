using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Legion
{
    class HungryNanobladesCardController : CardController
    {
        public HungryNanobladesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = DealDamage(CharacterCard, (Card c) => IsHero(c), 2, DamageType.Melee, true, addStatusEffect: (DealDamageAction dd) => IncreaseVictimDamageTaken(dd));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator IncreaseVictimDamageTaken(DealDamageAction dd)
        {
            if (dd.DidDealDamage && !dd.DidDestroyTarget) {
                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
                idse.TargetCriteria.IsSpecificCard = dd.Target;
                idse.UntilEndOfNextTurn(TurnTaker);
                IEnumerator coroutine = AddStatusEffect(idse);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                IEnumerator coroutine = DoNothing();
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
