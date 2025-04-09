using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.Nyctophobia
{
    internal class ShadowStepCardController : CardController
    {
        public ShadowStepCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            OnDealDamageStatusEffect oddse = new OnDealDamageStatusEffect(this.CardWithoutReplacements, "CancelDamage", "Prevent the next damage dealt to Nyctophobia.", new[] { TriggerType.WouldBeDealtDamage }, this.TurnTaker, this.Card, new int[] { });
            oddse.TargetCriteria.IsSpecificCard = CharacterCard;
            oddse.NumberOfUses = 1;
            oddse.BeforeOrAfter = BeforeOrAfter.Before;

            IEnumerator coroutine = GameController.AddStatusEffectEx(oddse, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public IEnumerator CancelDamage(DealDamageAction dd, HeroTurnTaker htt, StatusEffect se, int[] powerNumerals = null)
        {
            IEnumerator coroutine = CancelActionEx(dd, isPreventEffect: true);
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
