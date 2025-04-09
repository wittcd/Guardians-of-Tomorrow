using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Kumiho
{
    class GhoulishHuntersCardController : CardController
    {
        public GhoulishHuntersCardController(Card card, TurnTakerController turnTakerController)
               : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageAndDoExtraOnKillResponse, TriggerType.DealDamage);
            base.AddTriggers();
        }

        private IEnumerator DealDamageAndDoExtraOnKillResponse(PhaseChangeAction pc)
        {
            /*List<DealDamageAction> storedResultsA = new List<DealDamageAction>();
            List<DealDamageAction> storedResultsB = new List<DealDamageAction>();*/
            List<DealDamageAction> damages = new List<DealDamageAction>();
            List<DealDamageAction> damages2 = new List<DealDamageAction>();
            /*damages.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, base.Card), null, 1, DamageType.Melee, false, null, storedResultsA, null, false));
            damages.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, base.Card), null, 1, DamageType.Cold, false, null, storedResultsB, null, false));*/

            //IEnumerator coroutine = DealMultipleInstancesOfDamageToHighestLowestHP(damages, (Card c) => IsHero(c), HighestLowestHP.LowestHP, 1, 1);
            IEnumerator coroutine = DealDamageToLowestHP(Card, 1, (Card c) => IsHero(c), (Card c) => 1, DamageType.Melee, storedResults: damages);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(damages))
            {
                int flag = 0;
                foreach (DealDamageAction dd in damages)
                {
                    if (dd.DidDestroyTarget) 
                    {
                        flag = 1;
                    }
                }
                if (flag == 1)
                {
                    coroutine = GameController.SendMessageAction("Ghoulish Hunters heals itself and plays the top card of the villain deck.", Priority.Medium, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    coroutine = GameController.SetHP(base.Card, 5, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = PlayTheTopCardOfTheVillainDeckResponse(null);
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
                    coroutine = DealDamage(Card, damages.FirstOrDefault().OriginalTarget, 1, DamageType.Cold, storedResults: damages2, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    foreach (DealDamageAction dd in damages2)
                    {
                        if (dd.DidDestroyTarget)
                        {
                            flag = 1;
                        }
                    }
                    if (flag == 1)
                    {
                        /*coroutine = GameController.SendMessageAction("Ghoulish Hunters heals itself and plays the top card of the villain deck.", Priority.Medium, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }*/
                        coroutine = GameController.SetHP(base.Card, 5, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        coroutine = PlayTheTopCardOfTheVillainDeckResponse(null);
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
            else
            {
                coroutine = DoNothing();
            }
        }
    }
}
