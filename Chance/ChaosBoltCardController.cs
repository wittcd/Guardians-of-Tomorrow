using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class ChaosBoltCardController : ChanceCardController
    {

        public ChaosBoltCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<SelectDamageTypeDecision> resultType = new List<SelectDamageTypeDecision>();
            IEnumerator coroutine = GameController.SelectDamageType(DecisionMaker, resultType, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            DamageType selected = GetSelectedDamageType(resultType).Value;

            List<DealDamageAction> damageList = new List<DealDamageAction>();

            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), 3, selected, 1, false, 1, storedResultsDamage: damageList, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(damageList))
            {
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
                    List<SelectDamageTypeDecision> resultType2 = new List<SelectDamageTypeDecision>();
                    coroutine = GameController.SelectDamageType(DecisionMaker, resultType2, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    selected = GetSelectedDamageType(resultType2).Value;

                    if (coin.FirstOrDefault() == 1)
                    {
                        coroutine = GameController.DealDamage(DecisionMaker, base.CharacterCard, (Card c) => c == damageList.FirstOrDefault().Target, 3, selected, cardSource: GetCardSource());
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
                        coroutine = GameController.DealDamage(DecisionMaker, base.CharacterCard, (Card c) => !IsHero(c) && c != damageList.FirstOrDefault().Target, 1, selected, cardSource: GetCardSource());
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
}
