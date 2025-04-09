using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using GuardiansOfTomorrow.TheReverseEngineer;

namespace GuardiansOfTomorrow.TheReverseEngineer
{
    class TransdimensionalBladeCardController : TheReverseEngineerCardController
    {
        public TransdimensionalBladeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator FromTrashAction()
        {
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Melee, 3, false, 0, true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeralTargets = GetPowerNumeral(0, 1);
            int powerNumeralDamage = GetPowerNumeral(1, 2);
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), powerNumeralDamage, DamageType.Melee, powerNumeralTargets, false, powerNumeralTargets, true, cardSource: GetCardSource());
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
