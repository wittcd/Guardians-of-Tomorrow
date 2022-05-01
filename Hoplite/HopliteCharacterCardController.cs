using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Hoplite
{
    public class HopliteCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public HopliteCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //select target, reduce damage to and by by 2 until end of your next turn.
            int reduceAmount = GetPowerNumeral(0, 2);
            List<SelectCardDecision> selectedTargets = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.ReduceDamageTaken, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay, "target"), selectedTargets, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (selectedTargets.FirstOrDefault() != null) 
            {
                Card t = selectedTargets.FirstOrDefault().SelectedCard;
                ReduceDamageStatusEffect reduceIncoming = new ReduceDamageStatusEffect(reduceAmount);
                ReduceDamageStatusEffect reduceOutgoing = new ReduceDamageStatusEffect(reduceAmount);

                reduceIncoming.TargetCriteria.IsSpecificCard = t;
                reduceOutgoing.SourceCriteria.IsSpecificCard = t;
                reduceIncoming.CardDestroyedExpiryCriteria.Card = base.CharacterCard;
                reduceOutgoing.CardDestroyedExpiryCriteria.Card = base.CharacterCard;
                reduceIncoming.UntilEndOfNextTurn(base.TurnTaker);
                reduceOutgoing.UntilEndOfNextTurn(base.TurnTaker);

                coroutine = GameController.AddStatusEffect(reduceOutgoing, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.AddStatusEffect(reduceIncoming, true, GetCardSource());
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //draw
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
                        //play
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
                case 2:
                    {
                        //reduce next damage to or by target by 2
                        List<SelectCardDecision> selectedTargets = new List<SelectCardDecision>();
                        IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.ReduceDamageTaken, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay, "target"), selectedTargets, false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (selectedTargets.FirstOrDefault() != null)
                        {
                            Card t = selectedTargets.FirstOrDefault().SelectedCard;
                            ReduceDamageStatusEffect reduceOutgoing = new ReduceDamageStatusEffect(2);

                            reduceOutgoing.SourceCriteria.IsSpecificCard = t;
                            reduceOutgoing.NumberOfUses = 1;

                            coroutine = GameController.AddStatusEffect(reduceOutgoing, true, GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
            }
        }
    }
}