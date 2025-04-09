using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheSmiler
{
    public class TheSmilerCharacterCardController : VillainCharacterCardController
    {
        public TheSmilerCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            
        }

        public override void AddTriggers()
        {
            AddDefeatedIfDestroyedTriggers(false);
            //Increase damage dealt by {TheSmiler} to hero targets by the number of Clues in their play area. 
            AddIncreaseDamageTrigger((DealDamageAction dd) => IsHeroTarget(dd.Target) && dd.DamageSource.Card == CharacterCard, (DealDamageAction dd) => FindCardsWhere((Card c) => c.IsAtLocationRecursive(dd.Target.Location.OwnerTurnTaker.PlayArea) && c.IsClue).Count());
            
            if (IsGameChallenge)
            {
                AddReduceDamageToSetAmountTrigger((DealDamageAction dd) => dd.Target == CharacterCard, FindCardsWhere((Card c) => c.IsInPlay && c.IsClue).Count());
            }

            base.AddTriggers();
        }

        public override void AddSideTriggers()
        {
            if (!Card.IsFlipped)
            {
                //{TheSmiler} is immune to damage from hero targets without a Clue in their play area. 
                AddSideTrigger(AddImmuneToDamageTrigger((DealDamageAction dd) => (dd.Target == CharacterCard && IsHeroTarget(dd.DamageSource.Card) && !(FindCardsWhere((Card c) => c.IsClue && c.IsAtLocationRecursive(dd.DamageSource.Card.Location.OwnerTurnTaker.PlayArea)).Count() > 0))));
                //After {TheSmiler} is dealt damage by a hero target, flip this card.
                AddSideTrigger(AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.Target == CharacterCard && dd.DamageSource.IsHero, (DealDamageAction dd) => FlipThisCharacterCardResponse(null), TriggerType.FlipCard, TriggerTiming.After));
                //At the end of the villain turn, {TheSmiler} deals each hero target 1 psychic damage.
                AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => IsHeroTarget(c), 2, DamageType.Psychic, true, cardSource: GetCardSource()), TriggerType.DealDamage));

                //"advanced": "Redirect damage dealt by environment cards to {TheSmiler} to the hero with the most cards in their play area."
                if (IsGameAdvanced)
                {
                    AddSideTrigger(AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource.IsEnvironmentCard && dd.Target == CharacterCard, RedirectToHeroWithMostCards, TriggerType.RedirectDamage, TriggerTiming.Before));
                }
            }
            else 
            {
                //Whenever a Clue enters play, destroy it and play the top card of the villain deck.
                AddSideTrigger(AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cep) => cep.CardEnteringPlay.IsClue, DestroyPlayedCardAndPlayTopCard, TriggerType.DestroyCard, TriggerTiming.After));
                //At the end of the villain turn, {TheSmiler} deals the hero with the most Clues in their play area {H} infernal damage, then destroys one Clue in their play area.
                //Then, if there are no Clues in play, shuffle the villain trash into the villain deck and flip this card.
                AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => FlippedEndOfTurnRoutine(), new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroyCard }));

                //"flippedAdvanced": "At the end of the villain turn, {TheSmiler} deals each non-villain, non-character target 2 psychic damage. If any targets are destroyed this way, each hero must discard a card.",
                if (IsGameAdvanced)
                {
                    AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DamageNonCharacters_DiscardIfDestroy(), TriggerType.DealDamage));
                }
            }
        }

        private IEnumerator RedirectToHeroWithMostCards(DealDamageAction dd) 
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = FindHeroTargetWithMostCardsInPlay(storedResults, SelectionType.RedirectDamage, 1, 1, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard, "hero character"), null, null, true, false, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectTargetAndRedirectDamage(DecisionMaker, (Card c) => storedResults.Contains(c), dd, false, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DestroyPlayedCardAndPlayTopCard(CardEntersPlayAction cep) 
        {
            IEnumerator coroutine = base.GameController.DestroyCard(DecisionMaker, cep.CardEnteringPlay, optional: false, null, null, null, null, null, null, null, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.PlayTopCard(DecisionMaker, TurnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator FlippedEndOfTurnRoutine()
        {
            List<TurnTaker> most = new List<TurnTaker>();
            IEnumerator coroutine = FindHeroWithMostCardsInPlayArea(most, 1, 1, null, new LinqCardCriteria((Card c) => c.IsClue, "clue"));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<DealDamageAction> storedDamage = new List<DealDamageAction>();
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), H, DamageType.Infernal, 1, false, 1, false, true, false, (Card c) => most.Contains(c.Owner) && c.IsCharacter, storedResultsDamage: storedDamage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsClue && c.IsAtLocationRecursive(storedDamage.FirstOrDefault().OriginalTarget.Owner.PlayArea), "clue"), false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (FindCardsWhere((Card c) => c.IsClue && c.IsInPlay).Count() == 0) 
            {
                coroutine = GameController.ShuffleTrashIntoDeck(TurnTakerController, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = FlipThisCharacterCardResponse(null);
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

        private IEnumerator DamageNonCharacters_DiscardIfDestroy() 
        {
            List<DealDamageAction> damages = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => !IsVillain(c) && !c.IsCharacter, 2, DamageType.Psychic, true, false, damages, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            bool destroyed = false;
            foreach (DealDamageAction action in damages)
            {
                if (action.DidDestroyTarget)
                {
                    destroyed = true;
                }
            }
            if (destroyed)
            {
                coroutine = GameController.EachPlayerDiscardsCards(1, 1, cardSource: GetCardSource());
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
