using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class IncendiaryMissilesCardController : TLMHWeaponCardController
    {
        public IncendiaryMissilesCardController(Card card, TurnTakerController turnTakerController)
                    : base(card, turnTakerController)
        {
        }

        public override IEnumerator ActivateAttack()
        {
            IEnumerator coroutine = DealDamage(CharacterCard, (Card c) => !IsVillain(c), 2, DamageType.Fire);
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
