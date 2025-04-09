using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Ludramor
{
    class IntelligenceOfficerSalguulCardController : CardController
    {
        public static readonly string FirstDiscardKey = "SalguulTriggeredDiscard";
        public IntelligenceOfficerSalguulCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("analyst"), "analyst"));
        }

        private IEnumerator RedirectToAnalyst(DealDamageAction dd)
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card card) => card != Card && card.DoKeywordsContain("analyst"), storedResults, dd, null, evenIfCannotDealDamage: false, optional: false, null, ignoreBattleZone: false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card2 = storedResults.FirstOrDefault();
            if (card2 != null)
            {
                IEnumerator coroutine2 = base.GameController.RedirectDamage(dd, card2, isOptional: false, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
        }

        private int numAnalyst()
        {
            return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("analyst")).Count();
        }

        public override void AddTriggers()
        {
            AddTrigger((DealDamageAction dd) => dd.Target == Card, RedirectToAnalyst, TriggerType.RedirectDamage, TriggerTiming.Before);
            AddStartOfTurnTrigger((TurnTaker tt) => true, (PhaseChangeAction pc) => reset_bonus_discard(), TriggerType.Other);
            AddTrigger((DiscardCardAction dc) => IsHero(dc.CardToDiscard), addExtraDiscardFirstTime, TriggerType.DiscardCard, TriggerTiming.After);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => GameController.EachPlayerDiscardsCards(1, 1, cardSource: GetCardSource()), TriggerType.DiscardCard);
        }

        private IEnumerator addExtraDiscardFirstTime(DiscardCardAction dc)
        {
            if (GameController.GetCardPropertyJournalEntryBoolean(Card, FirstDiscardKey).Value)
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
            else
            {
                GameController.AddCardPropertyJournalEntry(Card, FirstDiscardKey, true);
                IEnumerator coroutine = GameController.SelectAndDiscardCards(FindTurnTakerController(dc.CardToDiscard.Owner).ToHero(), numAnalyst() - 1, false, numAnalyst(), cardSource: GetCardSource());
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

        private IEnumerator reset_bonus_discard()
        {
            GameController.AddCardPropertyJournalEntry(Card, FirstDiscardKey, false);
            yield break;
        }
    }
}
