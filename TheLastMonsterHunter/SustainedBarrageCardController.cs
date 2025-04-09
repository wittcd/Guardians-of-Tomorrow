using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class SustainedBarrageCardController : CardController
    {
        public SustainedBarrageCardController(Card card, TurnTakerController turnTakerController)
                    : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<Card> highest = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithHighestHitPoints(1, (Card c) => c.DoKeywordsContain("weapon"), highest, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidFindCard(highest))
            {
                Card weapon = highest.FirstOrDefault();
                if (weapon.HasActivatableAbility("attack"))
                {
                    coroutine = ActivateWeaponAttack(weapon);
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

            coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => H - 1, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator ActivateWeaponAttack(Card weapon)
        {
            TLMHWeaponCardController weaponCardController = FindCardController(weapon) as TLMHWeaponCardController;
            if (weapon.IsInPlay)
            {
                string key = "attack";
                IEnumerator coroutine = base.GameController.ActivateAbility(weaponCardController.GetActivatableAbilities(key).First(), GetCardSource());
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
