using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Ludramor
{
    class LudramorCharacterCardController : VillainCharacterCardController
    {
        public LudramorCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => FindAllTargetsWithLowestHitPoints((Card c2) => c2 != CharacterCard && c2.DoKeywordsContain("executive"), 1).Contains(c) && !CharacterCard.IsFlipped, "other executive"));
        }

        public override void AddTriggers()
        {
            //base.AddTriggers();
            AddDefeatedIfDestroyedTriggers(false);
            TriggerType[] types = { TriggerType.DealDamage, TriggerType.RemoveFromGame };
            if (IsGameChallenge)
            {
                AddImmuneToDamageTrigger((DealDamageAction dd) => dd.Target.Identifier == "PlanetcrackerBarge");
                AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), false, cardSource: GetCardSource()), TriggerType.DestroyCard);
            }
            base.AddTriggers();
        }

        public override void AddSideTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                AddSideTrigger(AddTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard, RedirectToExecutive, TriggerType.RedirectDamage, TriggerTiming.Before));
                AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, FlipIfNoExecs, TriggerType.FlipCard));
                if (IsGameAdvanced)
                {
                    AddSideTrigger(AddReduceDamageTrigger((Card c) => c.DoKeywordsContain("executive"), 1));
                }
            }
            else
            {
                AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, FlipIfSufficientExecs, TriggerType.FlipCard));
                AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, FlippedEoT, TriggerType.PutIntoPlay));
                if (IsGameAdvanced)
                {
                    AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card.DoKeywordsContain("executive"), (DealDamageAction dd) => 1));
                }
            }
        }

        private IEnumerator FlipIfNoExecs(PhaseChangeAction pc)
        {
            if (GameController.FindTargetsInPlay((Card c) => c != CharacterCard && c.DoKeywordsContain("executive")).Count() == 0)
            {
                IEnumerator coroutine = GameController.ShuffleTrashIntoDeck(TurnTakerController, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = FlipThisCharacterCardResponse(pc);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            } else
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

        private IEnumerator FlipIfSufficientExecs(PhaseChangeAction pc)
        {
            if (GameController.FindTargetsInPlay((Card c) => IsVillain(c) && c != CharacterCard).Count() >= H - 1 && GameController.FindTargetsInPlay((Card c) => c.DoKeywordsContain("executive") && c != CharacterCard).Count() > 0)
            {
                IEnumerator coroutine = FlipThisCharacterCardResponse(pc);
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
        
        private IEnumerator RedirectToExecutive(DealDamageAction dd)
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card card) => card != CharacterCard && card.DoKeywordsContain("executive"), storedResults, dd, null, evenIfCannotDealDamage: false, optional: false, null, ignoreBattleZone: false, GetCardSource());
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

        private IEnumerator FlippedEoT(PhaseChangeAction pc)
        {
            IEnumerator coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, TurnTaker.Deck, true, true, false, new LinqCardCriteria((Card c) => c.IsTarget, "target"), 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = DealDamageToLowestHP(CharacterCard, 1, (Card c) => !IsVillain(c), (Card c) => H - 1, DamageType.Energy);
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
