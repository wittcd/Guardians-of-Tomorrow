using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Ninetails
{
    class NinetailsFlamePriestessCharacterCardController : HeroCharacterCardController
    {
        public NinetailsFlamePriestessCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("ghost")));
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeralTargetsRadiant = GetPowerNumeral(0, 1);
            int powerNumeralDamageRadiant = GetPowerNumeral(1, 1);
            int powerNumeralTargetsFire = GetPowerNumeral(2, 1);
            int powerNumeralDamageFire = GetPowerNumeral(3, 3);

            List<DealDamageAction> damages = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), powerNumeralDamageRadiant, DamageType.Radiant, powerNumeralTargetsRadiant, false, powerNumeralTargetsRadiant, storedResultsDamage: damages, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            int d = 0;
            foreach (DealDamageAction dd in damages)
            {
                if (dd.Target.DoKeywordsContain("ghost") && dd.DidDealDamage)
                {
                    d = 1;
                }
            }

            if (d == 1)
            {
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), powerNumeralDamageFire, DamageType.Fire, powerNumeralTargetsFire, false, powerNumeralTargetsFire, additionalCriteria: (Card c) => !IsHero(c), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource());
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
                        //use power
                        IEnumerator coroutine = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, GetCardSource());
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
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator coroutine = GameController.SelectHeroCharacterCard(DecisionMaker, SelectionType.IncreaseNextDamage, storedResults, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        Card SelectedHero = storedResults.FirstOrDefault().SelectedCard;
                        IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(2);
                        idse.NumberOfUses = 1;
                        idse.SourceCriteria.IsSpecificCard = SelectedHero;

                        ChangeDamageTypeStatusEffect cdtse = new ChangeDamageTypeStatusEffect(DamageType.Fire);
                        cdtse.NumberOfUses = 1;
                        cdtse.SourceCriteria.IsSpecificCard = SelectedHero;

                        coroutine = AddStatusEffect(idse);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        coroutine = AddStatusEffect(cdtse);
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
