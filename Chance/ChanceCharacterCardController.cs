using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class ChanceCharacterCardController : ChanceHeroCharacterCardController
    {
        public ChanceCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int PowerNumeralCards = GetPowerNumeral(0, 1);
            int PowerNumeralTargets = GetPowerNumeral(1, 1);
            int PowerNumeralDamage = GetPowerNumeral(2, 2);

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
                    coroutine = DrawCards(DecisionMaker, PowerNumeralCards);
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
                    List<SelectDamageTypeDecision> storedType = new List<SelectDamageTypeDecision>();
                    coroutine = GameController.SelectDamageType(DecisionMaker, storedType, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    DamageType type = GetSelectedDamageType(storedType).Value;

                    /*coroutine = GameController.SendMessageAction("DamageType: " + type.ToString(), Priority.Critical, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }*/


                    coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), PowerNumeralDamage, type, PowerNumeralTargets, false, PowerNumeralTargets, cardSource: GetCardSource());
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
                        IEnumerator coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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
                        IEnumerator coroutine = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
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
                                coroutine = GameController.SelectAndGainHP(DecisionMaker, 2, false, (Card c) => IsHeroCharacterCard(c), 1, null, false, null, GetCardSource());
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
                                List<SelectDamageTypeDecision> storedType = new List<SelectDamageTypeDecision>();
                                coroutine = GameController.SelectDamageType(DecisionMaker, storedType, cardSource: GetCardSource());
                                if (base.UseUnityCoroutines)
                                {
                                    yield return base.GameController.StartCoroutine(coroutine);
                                }
                                else
                                {
                                    base.GameController.ExhaustCoroutine(coroutine);
                                }
                                coroutine = GameController.SelectHeroToSelectTargetAndDealDamage(DecisionMaker, 2, storedType.FirstOrDefault().SelectedDamageType.Value, cardSource: GetCardSource());
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
