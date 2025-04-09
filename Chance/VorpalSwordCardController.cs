using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class VorpalSwordCardController : ChanceCardController
    {
        public VorpalSwordCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageType == DamageType.Melee && dd.DamageSource.Card == base.CharacterCard, 1);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int PowerNumeralTargets = GetPowerNumeral(0, 1);
            int PowerNumeralDamage = GetPowerNumeral(1, 2);
            int PowerNumeralFlips = GetPowerNumeral(2, 4);
            int PowerNumeralHeads = GetPowerNumeral(3, 4);

            List<DealDamageAction> storedResultDamage = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), PowerNumeralDamage, DamageType.Melee, PowerNumeralTargets, false, PowerNumeralTargets, storedResultsDamage: storedResultDamage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(storedResultDamage))
            {
                foreach (DealDamageAction dd in storedResultDamage)
                {
                    if (dd.DidDealDamage && !dd.Target.IsCharacter && !dd.DidDestroyTarget)
                    {
                        List<int> headCount = new List<int>();
                        coroutine = FlipMultipleTimesAndCountHeads(PowerNumeralFlips, headCount, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (headCount.Count() > 0 && headCount.FirstOrDefault() >= PowerNumeralHeads)
                        {
                            coroutine = GameController.SendMessageAction("Snicker-snack!", Priority.High, GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                            coroutine = GameController.DestroyCard(DecisionMaker, dd.Target, cardSource: GetCardSource());
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
        }
    }
}
