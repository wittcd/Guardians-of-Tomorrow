using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class ChaosArcCardController : ChanceCardController
    {
        public ChaosArcCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int PowerNumeralFlips = GetPowerNumeral(0, 2);
            int PowerNumeralTargets = GetPowerNumeral(1, 1);
            int PowerNumeralDamage = GetPowerNumeral(2, 1);
            int PowerNumeralAdd = GetPowerNumeral(3, 1);

            List<int> headCount = new List<int>();
            IEnumerator coroutine = FlipMultipleTimesAndCountHeads(PowerNumeralFlips, headCount, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (headCount.Count() > 0)
            {
                int x = headCount.FirstOrDefault() + PowerNumeralAdd;
                String singleDamageDesc = "Deal " + x + " damage to " + PowerNumeralTargets + " target";
                String multiDamageDesc = "Deal " + PowerNumeralDamage + " damage to up to " + x + " targets";
                List<SelectDamageTypeDecision> resultType = new List<SelectDamageTypeDecision>();
                coroutine = GameController.SelectDamageType(DecisionMaker, resultType, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                List<Function> damageOptions = new List<Function>();
                damageOptions.Add(new Function(DecisionMaker, singleDamageDesc, SelectionType.DealDamage, () => GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), x, resultType.FirstOrDefault().SelectedDamageType.Value, PowerNumeralTargets, false, PowerNumeralTargets, cardSource: GetCardSource())));
                damageOptions.Add(new Function(DecisionMaker, multiDamageDesc, SelectionType.DealDamage, () => GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), PowerNumeralDamage, resultType.FirstOrDefault().SelectedDamageType.Value, x, false, 0, cardSource: GetCardSource())));
                SelectFunctionDecision sfd = new SelectFunctionDecision(GameController, DecisionMaker, damageOptions, false, cardSource: GetCardSource());
                coroutine = GameController.SelectAndPerformFunction(sfd);
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
}
