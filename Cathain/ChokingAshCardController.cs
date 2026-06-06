using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace GuardiansOfTomorrow.Cathain
{
    public class ChokingAshCardController : CardController
    {
        public ChokingAshCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, increase the next damage dealt to each non-hero target by 1 and {Cathain} or one of your Loci deals itself 6 irreducible infernal damage.",
            IEnumerator coroutine = GameController.SelectCardsAndDoAction(DecisionMaker, new LinqCardCriteria((Card c) => !IsHero(c) && c.IsTarget && c.IsInPlayAndHasGameText, "non-hero"), SelectionType.IncreaseDamageTaken, IncreaseNextDamageTaken, allowAutoDecide: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
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
            //"The first time each turn each non-hero target deals damage to another target, they deal themselves 2 toxic damage.",
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource.IsCard && dd.DamageSource.Card.IsTarget && !IsHero(dd.DamageSource.Card) && dd.DamageSource.Card != dd.Target && !DidTargetAlreadyDealDamageToAnother(dd), (DealDamageAction dd) => DealDamage(dd.DamageSource.Card, dd.DamageSource.Card, 2, DamageType.Toxic, cardSource: GetCardSource()), TriggerType.DealDamage, TriggerTiming.After);
            //"At the end of your turn, if this card did not enter play this turn, destroy it."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DestroyThisCardResponse(null), TriggerType.DestroySelf, (PhaseChangeAction pc) => !DidEnterThisTurn(pc));
        }

        private bool DidTargetAlreadyDealDamageToAnother(DealDamageAction dd)
        {
            int c = 0;
            if (dd.DamageSource.IsCard)
            {
                List<DealDamageJournalEntry> entries = Journal.DealDamageEntriesThisTurnSinceCardWasPlayed(Card).ToList();
                foreach (DealDamageJournalEntry entry in entries)
                {
                    if (entry.SourceCard != entry.TargetCard && entry.SourceCard == dd.DamageSource.Card)
                    {
                        c += 1;
                    }
                }
            }

            if (c > 1)
            {
                return true; //because this happens *after* damage is dealt, there will always be at least 1. If it's more, that means damage has been dealt multiple times.
            }

            return false;
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

        private IEnumerator IncreaseNextDamageTaken(Card c)
        {
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.TargetCriteria.IsSpecificCard = c;
            idse.NumberOfUses = 1;
            IEnumerator coroutine = GameController.AddStatusEffectEx(idse, true, GetCardSource(), new Card[] {Card});
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
