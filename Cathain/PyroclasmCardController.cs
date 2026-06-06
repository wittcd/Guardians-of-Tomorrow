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
    public class PyroclasmCardController : CardController
    {
        public PyroclasmCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //{Cathain} deals each non-hero target 2 fire damage
            IEnumerator coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => !IsHeroTarget(c), 2, DamageType.Fire, cardSource: GetCardSource());
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
            //"The first time each turn each non-hero target is dealt non-fire damage, {Cathain} deals that target 1 fire damage.",
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageType != DamageType.Fire && !IsHeroTarget(dd.Target), DealDamageButOnlyOnce, TriggerType.DealDamage, TriggerTiming.After);
            //"At the end of your turn, if this card did not enter play this turn, destroy it."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DestroyThisCardResponse(null), TriggerType.DestroySelf, (PhaseChangeAction pc) => !DidEnterThisTurn(pc));
        }

        private IEnumerator DealDamageButOnlyOnce(DealDamageAction dd)
        {
            Card target = dd.Target;
            List<DealDamageJournalEntry> journalEntries = Journal.DealDamageEntriesThisTurn().ToList();
            int count = 0;
            foreach (DealDamageJournalEntry entry in journalEntries)
            {
                if (entry.TargetCard == target && entry.DamageType != DamageType.Fire)
                {
                    count++;
                }
            }

            if (count <= 1) //has to be <= 1 since the triggering action is in the count but hasn't been responded to
            {
                IEnumerator coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c == target, 1, DamageType.Fire, cardSource: GetCardSource());
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
