using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class ControlledChaosCardController : ChanceCardController
    {
        public ControlledChaosCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<SelectNumberDecision> numCards = new List<SelectNumberDecision>();
            IEnumerator coroutine = GameController.SelectNumber(DecisionMaker, SelectionType.DrawCard, 0, 3, false, false, null, numCards, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            int draws = numCards.FirstOrDefault().SelectedNumber.Value;

            for (int x = 0; x < draws; x++)
            {
                coroutine = DrawAndDoExtraResponse();
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

        private IEnumerator DrawAndDoExtraResponse()
        {
            IEnumerator coroutine = GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<int> coin = new List<int>();

            coroutine = FlipCoin(coin, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (coin.Count() > 0)
            {
                if (coin.FirstOrDefault() == 1)
                {
                    coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), 1, DamageType.Energy, 1, false, 1, cardSource: GetCardSource());
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
                    coroutine = GameController.SelectAndDiscardCard(DecisionMaker, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, false, false, false, null, new LinqTurnTakerCriteria((TurnTaker tt) => tt != base.TurnTaker && tt.IsHero), null, GetCardSource());
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
}
