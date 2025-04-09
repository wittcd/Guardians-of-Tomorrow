using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class AggressiveHeatBleedCardController : CardController
    {
        public AggressiveHeatBleedCardController(Card card, TurnTakerController turnTakerController)
                    : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            if (CharacterCard.FindTokenPool("HeatPool").CurrentValue == 0)
            {
                IEnumerator coroutine = GameController.PlayTopCard(DecisionMaker, TurnTakerController, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            if (CharacterCard.FindTokenPool("HeatPool").CurrentValue >= 5)
            {
                IEnumerator coroutine = DealDamage(CharacterCard, (Card c) => IsHeroTarget(c), 2, DamageType.Fire);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            if (CharacterCard.FindTokenPool("HeatPool").CurrentValue >= 10)
            {
                IEnumerator coroutine = DealDamage(CharacterCard, CharacterCard, 3, DamageType.Fire);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.PlayTopCard(DecisionMaker, TurnTakerController, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            IEnumerator coroutine2 = GameController.RemoveTokensFromPool(CharacterCard.FindTokenPool("HeatPool"), H, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
        }
    }
}
