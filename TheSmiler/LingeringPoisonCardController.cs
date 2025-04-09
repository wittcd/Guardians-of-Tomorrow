using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheSmiler
{
    public class LingeringPoisonCardController : CardController
    {
        public LingeringPoisonCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            //{TheSmiler} deals each non-villain target 1 toxic damage.
            IEnumerator coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => !IsVillain(c), 2, DamageType.Toxic, addStatusEffect: DamagedTargetDiscards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
        private IEnumerator DamagedTargetDiscards(DealDamageAction dd)
        {
            //Whenever a hero is dealt damage this way, their player must discard a card.
            if (dd.DidDealDamage && IsHeroCharacterCard(dd.Target))
            {
                IEnumerator coroutine = GameController.SelectAndDiscardCard(FindHeroTurnTakerController(dd.Target.Owner.ToHero()), cardSource: GetCardSource());
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
