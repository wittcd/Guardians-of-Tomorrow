using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using GuardiansOfTomorrow.TheReverseEngineer;

namespace GuardiansOfTomorrow.TheReverseEngineer
{
    class UnstableAmplifierCardController : TheReverseEngineerCardController
    {
        public UnstableAmplifierCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator FromTrashAction()
        {
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.TargetCriteria.IsVillain = true;
            idse.UntilStartOfNextTurn(GameController.FindNextTurnTaker());

            IEnumerator coroutine = GameController.AddStatusEffect(idse, true, GetCardSource());
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
            int powerNumeralInitialDamage = GetPowerNumeral(0, 1);
            int powerNumeralExtraDamage = GetPowerNumeral(1, 1);
            //IEnumerator coroutine = DealDamage(CharacterCard, (Card c) => !c.IsHero, powerNumeralInitialDamage, DamageType.Sonic, addStatusEffect: (DealDamageAction dd) => IfDestroyedDamageHighestNonhero(dd, powerNumeralExtraDamage));
            SelectTargetsDecision decision = new SelectTargetsDecision(GameController, DecisionMaker, (Card c) => !IsHero(c) && c.IsInPlayAndNotUnderCard && c.IsTarget, null, false, null, true, new DamageSource(GameController, CharacterCard), 1, DamageType.Sonic, false, null, null, false, null, null, SelectionType.SelectTarget, null, null, null, true, GetCardSource());
            IEnumerator coroutine = GameController.SelectCardsAndDoAction(decision, DealDamageWithPotentialEcho, null, null, GetCardSource());
            
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        /*private IEnumerator IfDestroyedDamageHighestNonhero(DealDamageAction dd, int d)
        {
            if (dd.DidDestroyTarget)
            {
                IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => !c.IsHero, (Card c) => 1, DamageType.Sonic);
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

        private IEnumerator DealDamageWithPotentialEcho(SelectCardDecision selected)
        {
            Card target = selected.SelectedCard;
            List<DealDamageAction> result = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamage(CharacterCard, target, 1, DamageType.Sonic, storedResults: result, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (result.Any((DealDamageAction dd) => dd.DidDestroyTarget)) {
                coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => !IsHero(c), (Card c) => 1, DamageType.Sonic);
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
