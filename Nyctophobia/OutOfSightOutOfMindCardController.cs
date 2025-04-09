using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Nyctophobia
{
    internal class OutOfSightOutOfMindCardController : CardController
    {
        public static readonly string dealtdamagethisturnkey = "NycDealtDamageThisTurn";
        public OutOfSightOutOfMindCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowDamageDealt(new LinqCardCriteria((Card c) => c == CharacterCard, "Nyctophobia", false), thisTurn: true);
        }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, ResetNycDealtDamage, TriggerType.Other);
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard, SetNycDealtDamage, TriggerType.Other, TriggerTiming.After);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, addStatusEffects, TriggerType.CreateStatusEffect);
        }

        private IEnumerator SetNycDealtDamage(DealDamageAction dd)
        {
            AddCardPropertyJournalEntry(dealtdamagethisturnkey, true);
            IEnumerator coroutine = DoNothing();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator ResetNycDealtDamage(PhaseChangeAction pc)
        {
            AddCardPropertyJournalEntry(dealtdamagethisturnkey, false);
            IEnumerator coroutine = DoNothing();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private bool DidNycDealDamageThisTurn()
        {
            return GetCardPropertyJournalEntryBoolean(dealtdamagethisturnkey).Value == true;
        }

        private IEnumerator addStatusEffects(PhaseChangeAction pc)
        {
            if (!DidNycDealDamageThisTurn())
            {
                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
                idse.SourceCriteria.IsSpecificCard = CharacterCard;
                idse.NumberOfUses = 1;
                ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                rdse.TargetCriteria.IsSpecificCard = CharacterCard;
                rdse.UntilStartOfNextTurn(TurnTaker);

                IEnumerator coroutine = GameController.AddStatusEffectEx(idse, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.AddStatusEffectEx(rdse, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                IEnumerator coroutine = DoNothing();
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
