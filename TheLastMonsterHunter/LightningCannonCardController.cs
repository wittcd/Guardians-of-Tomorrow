using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class LightningCannonCardController : TLMHWeaponCardController
    {
        public LightningCannonCardController(Card card, TurnTakerController turnTakerController)
                    : base(card, turnTakerController)
        {
        }

        public override IEnumerator ActivateAttack()
        {
            //{TheLastMonsterHunter} deals each target in the hero play area with the most cards {H - 1} lightning damage, then this card deals {TheLastMonsterHunter} 2 fire damage.
            List<TurnTaker> foundheroes = new List<TurnTaker>();
            IEnumerator coroutine = FindHeroWithMostCardsInPlay(foundheroes);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (foundheroes.FirstOrDefault() != null) {
                coroutine = DealDamage(CharacterCard, (Card c) => c.IsAtLocationRecursive(foundheroes.FirstOrDefault().PlayArea), H, DamageType.Lightning);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
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
