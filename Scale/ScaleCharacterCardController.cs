using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Scale
{
    public class ScaleCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public ScaleCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //destroy balance
            LinqCardCriteria isBalance = new LinqCardCriteria((Card c) => c.DoKeywordsContain("balance"));
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, isBalance, true, null, base.CharacterCard, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            // Draw 1 card
            var numberOfCards = GetPowerNumeral(0, 1);
            IEnumerator e = DrawCards(this.HeroTurnTakerController, numberOfCards);

            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //one hero may use a power
                        IEnumerator coroutine = base.GameController.SelectHeroToUsePower(DecisionMaker, false, true, false, null, null, null, true, true, GetCardSource());
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
                        // one target gains 2 hp
                        IEnumerator coroutine = base.GameController.SelectAndGainHP(DecisionMaker, 2, false, null, 1, null, false, null, GetCardSource());
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
                        // increase or reduce all damage by 1 until start of turn
                        IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
                        idse.UntilStartOfNextTurn(base.TurnTaker);
                        ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                        rdse.UntilStartOfNextTurn(base.TurnTaker);

                        List<Function> list = new List<Function>();
                        Function item = new Function(DecisionMaker, "Increase all damage by 1", SelectionType.IncreaseDamage, () => AddStatusEffect(idse), null, null, "Increase all damage by 1");
                        list.Add(item);
                        Function item2 = new Function(DecisionMaker, "Reduce all damage by 1", SelectionType.ReduceDamageTaken, () => AddStatusEffect(rdse), null, null, "Reduce all damage by 1");
                        list.Add(item2);

                        SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, DecisionMaker, list, optional: false, null, null, null, GetCardSource());
                        IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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
            }
        }
    }
}