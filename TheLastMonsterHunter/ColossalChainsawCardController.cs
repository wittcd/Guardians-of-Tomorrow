using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class ColossalChainsawCardController : TLMHWeaponCardController
    {
        public ColossalChainsawCardController(Card card, TurnTakerController turnTakerController)
                    : base(card, turnTakerController)
        {
        }

        public override IEnumerator ActivateAttack()
        {
            //{TheLastMonsterHunter} deals the 2 hero targets with the lowest HP {H - 1} melee damage each, then this card deals {TheLastMonsterHunter} 2 fire damage.
            IEnumerator coroutine = DealDamageToLowestHPEx(CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => H - 1, DamageType.Melee, numberOfTargets: () => 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DealDamage(Card, CharacterCard, 2, DamageType.Fire, cardSource: GetCardSource());
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
