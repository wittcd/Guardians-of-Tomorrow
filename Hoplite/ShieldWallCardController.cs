using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Hoplite
{
    class ShieldWallCardController : CardController
    {
        public ShieldWallCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            List<Card> storedResults = new List<Card>();
            DealDamageAction gameAction = new DealDamageAction(base.GameController, null, base.CharacterCard, 2, DamageType.Melee);
            IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card card) => IsVillainTarget(card), storedResults, gameAction, null, evenIfCannotDealDamage: false, optional: false, null, ignoreBattleZone: false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card2 = storedResults.FirstOrDefault();
            if (card2 != null)
            {
                IEnumerator coroutine2 = DealDamage(card2, base.CharacterCard, 2, DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }

            ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(2);
            rdse.TargetCriteria.IsHero = true;
            rdse.TargetCriteria.IsNotSpecificCard = base.CharacterCard;
            rdse.UntilStartOfNextTurn(base.TurnTaker);
            coroutine = GameController.AddStatusEffect(rdse, true, GetCardSource());
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
