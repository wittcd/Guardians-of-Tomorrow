using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Cathain
{
    public class AntipodeCardController : CardController
    {
        public AntipodeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Cathain} deals one target 3 cold or fire damage, then {Cathain} or one of your Loci deals itself 6 irreducible infernal damage.",
            List<SelectDamageTypeDecision> choice = new List<SelectDamageTypeDecision>();
            IEnumerator coroutine = GameController.SelectDamageType(DecisionMaker, choice, new DamageType[] { DamageType.Cold, DamageType.Fire }, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 3, choice.FirstOrDefault().SelectedDamageType.Value, 1, false, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{Cathain} or one of your Loci deals itself 6 irreducible infernal damage.
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 6, DamageType.Infernal, 1, false, 1, additionalCriteria: (Card c) => c == CharacterCard, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            //"The first time each turn a non-hero target takes cold damage, {Cathain} deals that target 3 irreducible fire damage.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => !IsHeroTarget(dd.Target) && (dd.DamageType == DamageType.Cold || dd.DamageType == DamageType.Fire) && GameController.GetCardPropertyJournalEntryBoolean(Card, "cathain_antipode_trigger").Value != true, DealOtherTypeDamage, TriggerType.DealDamage, TriggerTiming.After);
            AddEndOfTurnTrigger((TurnTaker tt) => true, (PhaseChangeAction pc) => ResetTracker(), TriggerType.Other);
            //"At the end of your turn, if this card did not enter play this turn, destroy it."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DestroyThisCardResponse(null), TriggerType.DestroySelf, (PhaseChangeAction pc) => !DidEnterThisTurn(pc));
        }

        private int NumColdOrFireTaken()
        {
            List<DealDamageJournalEntry> ddje = Journal.DealDamageEntriesThisTurn().ToList();
            int count = 0;

            foreach (DealDamageJournalEntry entry in ddje)
            {
                if ((entry.DamageType == DamageType.Cold || entry.DamageType == DamageType.Fire) && !IsHeroTarget(entry.TargetCard))
                {
                    count++;
                }
            }

            return count;
        }

        private IEnumerator ResetTracker()
        {
            GameController.AddCardPropertyJournalEntry(Card, "cathain_antipode_trigger", false);
            IEnumerator coroutine = GameController.DoNothing();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private DamageType OtherDamageType(DamageType dt)
        {
            if (dt == DamageType.Cold)
            {
                return DamageType.Fire;
            }
            return DamageType.Cold;
        }

        private IEnumerator DealOtherTypeDamage(DealDamageAction dd)
        {
            bool triggered = GameController.GetCardPropertyJournalEntryBoolean(Card, "cathain_antipode_trigger").Value;

            if (triggered != true)
            {
                GameController.AddCardPropertyJournalEntry(Card, "cathain_antipode_trigger", true);

                IEnumerator coroutine1 = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c == dd.Target, 3, OtherDamageType(dd.DamageType), isIrreducible: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine1);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine1);
                }
            }
            IEnumerator coroutine2 = GameController.DoNothing();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
        }

        private bool DidEnterThisTurn(PhaseChangeAction pc)
        {
            List<CardEntersPlayJournalEntry> entries = Journal.CardEntersPlayEntriesThisTurn().ToList();
            foreach (CardEntersPlayJournalEntry entry in entries)
            {
                if (entry.Card == Card)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
