using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class FieldRefitCardController : CardController
    {
        public FieldRefitCardController(Card card, TurnTakerController turnTakerController)
                    : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Reveal cards from the villain deck until a Weapon is revealed. Put it into play and shuffle the remaining cards back into the villain deck. Activate its Attack text and reduce damage dealt to it by 1 until the start of the villain turn."
            List<Card> playedCards = new List<Card>();
            IEnumerator coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("weapon"), "weapon"), 1, storedPlayResults: playedCards);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (playedCards.Count > 0)
            {
                Card played = playedCards.FirstOrDefault();
                if (played.HasActivatableAbility("attack"))
                {
                    coroutine = ActivateWeaponAttack(played);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                if (played.IsTarget)
                {
                    ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                    rdse.TargetCriteria.IsSpecificCard = played;
                    rdse.UntilStartOfNextTurn(TurnTaker);

                    coroutine = AddStatusEffectEx(rdse);
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
