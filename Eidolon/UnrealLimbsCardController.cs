using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class UnrealLimbsCardController : AlterationCardController
    {
        public UnrealLimbsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = AlterationPlayedResponse(DamageType.Melee, PreventLowestHPDamageResponse());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
        
        private IEnumerator PreventLowestHPDamageResponse()
        {
            List<Card> lowestHero = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsHero, lowestHero, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidFindCard(lowestHero))
            {
                CannotDealDamageStatusEffect cddse = new CannotDealDamageStatusEffect();
                cddse.SourceCriteria.IsSpecificCard = lowestHero.FirstOrDefault();
                cddse.UntilStartOfNextTurn(base.TurnTaker);
                coroutine = AddStatusEffect(cddse);
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
