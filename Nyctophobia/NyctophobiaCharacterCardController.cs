using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Nyctophobia
{
    internal class NyctophobiaCharacterCardController: HeroCharacterCardController
    {
        public NyctophobiaCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //{Nyctophobia} deals 1 target 1 melee damage. If a target is destroyed this way, you may use 1 power.
            /*int PowerNumeralTargets = GetPowerNumeral(0, 1);
            int PowerNumeralDamage = GetPowerNumeral(1, 1);
            int PowerNumeralPowers = GetPowerNumeral(2, 1);
            List<DealDamageAction> ddlist = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), PowerNumeralDamage, DamageType.Melee, PowerNumeralTargets, false, PowerNumeralTargets, storedResultsDamage: ddlist, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            bool destroyed = false;
            foreach (DealDamageAction dd in ddlist)
            {
                if (!destroyed && dd.DidDestroyTarget)
                {
                    destroyed = true;
                    coroutine = GameController.SelectAndUsePower(DecisionMaker, true, null, PowerNumeralPowers, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }*/

            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.SourceCriteria.IsSpecificCard = CharacterCard;
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
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            /*
            "One player may play a card.",
            "One hero may use a power",
            "Destroy a target with 2 or less HP."
            */

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
                        IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsTarget && c.HitPoints <= 2, "target with 2 or less HP", false), false, cardSource: GetCardSource());
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
