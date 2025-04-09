using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using GuardiansOfTomorrow.TheReverseEngineer;

namespace GuardiansOfTomorrow.TheReverseEngineer
{
    class NaniteSludgeSprayerCardController : TheReverseEngineerCardController
    {
        public NaniteSludgeSprayerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator FromTrashAction()
        {
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), true, null, CharacterCard, cardSource: GetCardSource());
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
            int powerNumeralTargets = GetPowerNumeral(0, 2);
            int powerNumeralReduction = GetPowerNumeral(1, 2);
            IEnumerator coroutine = GameController.SelectCardsAndDoAction(new SelectCardsDecision(GameController, DecisionMaker, (Card c) => c.IsInPlay && c.IsTarget, SelectionType.Custom, powerNumeralTargets, false, 0, cardSource: GetCardSource()), (SelectCardDecision scd) => ReduceNextDamageByCard(scd.SelectedCard, powerNumeralReduction), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator ReduceNextDamageByCard(Card c, int r)
        {
            ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(r);
            rdse.SourceCriteria.IsSpecificCard = c;
            rdse.NumberOfUses = 1;
            IEnumerator coroutine = AddStatusEffect(rdse);
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
