using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    abstract class ChanceHeroCharacterCardController : HeroCharacterCardController
    {

        public ChanceHeroCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public IEnumerator FlipCoin(List<int> storedResultsCoin, bool verbose, CardSource cardSource)
        {
            // 0 is tails and 1 is heads
            if (FindCardsWhere((Card c) => c.Identifier == "LuckyPenny" && c.IsInPlayAndHasGameText).Count() >= 1)
            {
                List<String> headsOrTails = new List<string>();
                headsOrTails.Add("heads");
                headsOrTails.Add("tails");
                List<SelectWordDecision> storedWord = new List<SelectWordDecision>();
                IEnumerator coroutine = GameController.SelectWord(DecisionMaker, headsOrTails, SelectionType.MakeDecision, storedWord, true, null, cardSource);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (DidSelectWord(storedWord))
                {
                    if (storedWord.FirstOrDefault().SelectedWord == "tails")
                    {
                        storedResultsCoin.Add(0);
                        if (verbose)
                        {
                            coroutine = GameController.SendMessageAction("Lucky Penny caused the coin to land on tails!", Priority.High, GetCardSource());
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
                    else
                    {
                        storedResultsCoin.Add(1);
                        if (verbose)
                        {
                            coroutine = GameController.SendMessageAction("Lucky Penny caused the coin to land on heads!", Priority.High, GetCardSource());
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
                    coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.Identifier == "LuckyPenny" && c.IsInPlayAndHasGameText), false, cardSource: cardSource);
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
                    storedResultsCoin.Add(Game.RNG.Next(0, 2));
                    if (storedResultsCoin.FirstOrDefault() == 1 && verbose)
                    {
                        coroutine = GameController.SendMessageAction("The coin landed on heads!", Priority.High, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                    }
                    else if (verbose)
                    {
                        coroutine = GameController.SendMessageAction("The coin landed on tails!", Priority.High, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                    }
                    coroutine = DoNothing();
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
            else
            {
                storedResultsCoin.Add(Game.RNG.Next(0, 2));
                if (storedResultsCoin.FirstOrDefault() == 1 && verbose)
                {
                    IEnumerator coroutine2 = GameController.SendMessageAction("The coin landed on heads!", Priority.High, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine2);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine2);
                    }
                }
                else if (verbose)
                {
                    IEnumerator coroutine2 = GameController.SendMessageAction("The coin landed on tails!", Priority.High, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine2);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine2);
                    }
                }
                IEnumerator coroutine = DoNothing();
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

        public IEnumerator FlipMultipleTimesAndCountHeads(int flips, List<int> storedResultsCoin, CardSource cardSource)
        {
            //Storedresultscoin contains the number of heads flipped this way.
            int sum = 0;
            for (int x = 0; x < flips; x++)
            {
                List<int> result = new List<int>();
                IEnumerator coroutine2 = FlipCoin(result, false, cardSource);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
                if (result != null)
                {
                    sum += result.FirstOrDefault();
                }
            }
            storedResultsCoin.Add(sum);
            IEnumerator coroutine = GameController.SendMessageAction("There were " + sum + " heads!", Priority.High, GetCardSource());
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
