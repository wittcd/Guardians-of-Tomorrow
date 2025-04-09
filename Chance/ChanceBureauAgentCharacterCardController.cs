using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Chance
{
    class ChanceBureauAgentCharacterCardController : ChanceHeroCharacterCardController
    {
        public ChanceBureauAgentCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeralEnv = GetPowerNumeral(0, 1);
            int powerNumeralOng = GetPowerNumeral(1, 1);

            List<int> result = new List<int>();
            IEnumerator coroutine = FlipCoin(result, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (result != null)
            {
                if (result.FirstOrDefault() == 1)
                {
                    coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), powerNumeralEnv, cardSource: GetCardSource());
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
                    coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => IsVillain(c) && IsOngoing(c), "villain ongoing"), powerNumeralOng, cardSource: GetCardSource());
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        IEnumerator coroutine = GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsTarget && c.HitPoints <= 3, "target with 3 or less HP"), false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 2:
                    {
                        List<int> storedcoin = new List<int>();
                        IEnumerator coroutine = FlipCoin(storedcoin, true, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (storedcoin != null && storedcoin.Count == 1)
                        {
                            if (storedcoin.FirstOrDefault() == 1)
                            {
                                ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(2);
                                rdse.NumberOfUses = 1;
                                rdse.TargetCriteria.IsHero = true;
                                coroutine = GameController.AddStatusEffectEx(rdse, true, GetCardSource());
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
                                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(2);
                                idse.NumberOfUses = 1;
                                idse.TargetCriteria.IsVillain = true;
                                coroutine = GameController.AddStatusEffectEx(idse, true, GetCardSource());
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

                        break;
                    }
            }
        }
    }
}
