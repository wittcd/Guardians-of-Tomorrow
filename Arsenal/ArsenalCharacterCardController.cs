using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace GuardiansOfTomorrow.Arsenal
{
    public class ArsenalCharacterCardController : HeroCharacterCardController
    {
        public ArsenalCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeralTargets = GetPowerNumeral(0, 1);
            int powerNumeralDamage = GetPowerNumeral(1, 1);

            DamageType[] types = { DamageType.Melee, DamageType.Projectile };
            List<SelectDamageTypeDecision> selectedType = new List<SelectDamageTypeDecision>();
            IEnumerator coroutine = GameController.SelectDamageType(DecisionMaker, selectedType, types, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), powerNumeralDamage, selectedType.FirstOrDefault().SelectedDamageType.Value, powerNumeralTargets, false, powerNumeralTargets, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        // One player may draw a card now.
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
                        // One hero may use a power
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
                        // Increase the next melee or projectile damage dealt by a hero by 2.
                        IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(2);
                        idse.SourceCriteria.IsHero = true;
                        idse.DamageTypeCriteria.AddType(DamageType.Melee);
                        idse.DamageTypeCriteria.AddType(DamageType.Projectile);
                        idse.NumberOfUses = 1;
                        IEnumerator coroutine = GameController.AddStatusEffectEx(idse, true, GetCardSource());
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
