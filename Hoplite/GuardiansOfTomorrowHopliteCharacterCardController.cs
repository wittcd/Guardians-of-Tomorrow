using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Hoplite
{
    public class GuardiansOfTomorrowHopliteCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public GuardiansOfTomorrowHopliteCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int times = GetPowerNumeral(0, 2);
            
            //redirect next from selected hero
            List<SelectCardDecision> selectCards = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.RedirectDamageDirectedAtTarget, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard, "Hero character card"), selectCards, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (selectCards.FirstOrDefault() != null) 
            {
                Card c = selectCards.FirstOrDefault().SelectedCard;
                RedirectDamageStatusEffect rdse = new RedirectDamageStatusEffect();
                rdse.TargetCriteria.IsSpecificCard = c;
                rdse.RedirectTarget = base.CharacterCard;
                rdse.NumberOfUses = times;
                coroutine = GameController.AddStatusEffect(rdse, true, GetCardSource());
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
                case 1:
                    {
                        //power
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
                        //redirect next damage dealt to A to B
                        List<SelectCardDecision> selectCards = new List<SelectCardDecision>();
                        IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.RedirectDamage, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard, "Hero character card"), selectCards, false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (selectCards.FirstOrDefault() != null)
                        {
                            Card origin = selectCards.FirstOrDefault().SelectedCard;
                            List<SelectCardDecision> secondSelect = new List<SelectCardDecision>();
                            coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.RedirectDamage, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c != origin, "other Hero character card"), secondSelect, false, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            if (secondSelect.FirstOrDefault() != null)
                            {
                                Card destination = secondSelect.FirstOrDefault().SelectedCard;
                                RedirectDamageStatusEffect rdse = new RedirectDamageStatusEffect();
                                rdse.TargetCriteria.IsSpecificCard = origin;
                                rdse.RedirectTarget = destination;
                                rdse.NumberOfUses = 1;
                                coroutine = GameController.AddStatusEffect(rdse, true, GetCardSource());
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