using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheHatter
{
    class WatchyourHeadsCardController : CardController
    {
        public WatchyourHeadsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        { }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = DealDamage(base.CharacterCard, (Card c) => IsHero(c), (Card c) => 1, DamageType.Projectile, addStatusEffect: RepeatDamageResponse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator RepeatDamageResponse(DealDamageAction dd)
        {
            Card target = dd.Target;
            IEnumerator coroutine;
            if (dd.DidDealDamage) {
                coroutine = DealDamage(base.CharacterCard, target, H, DamageType.Psychic, false, false, false, null, null, null, false, null);
            }
            else
            {
                coroutine = DoNothing();
            }
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
