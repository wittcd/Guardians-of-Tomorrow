using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller.SergeantSteelTeam;

namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class TheLastMonsterHunterCharacterCardController : VillainCharacterCardController
    {
        public TheLastMonsterHunterCharacterCardController(Card card, TurnTakerController turnTakerController)
                    : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //base.AddTriggers();
            AddDefeatedIfDestroyedTriggers(false);
            if (IsGameChallenge)
            {
                AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => GameController.RemoveTokensFromPool(CharacterCard.FindTokenPool("HeatPool"), H - 2, cardSource: GetCardSource()), TriggerType.ModifyTokens);
            }
            base.AddTriggers();
        }

        public override void AddSideTriggers()
        {
            if (!CharacterCard.IsFlipped)
            {
                if (IsGameAdvanced)
                {
                    // Reduce damage dealt to Upgrades and Weapons by 1.
                    AddSideTrigger(AddReduceDamageTrigger((Card c) => c.DoKeywordsContain("weapon") || c.DoKeywordsContain("upgrade"), 1));
                }
                // "Reduce non-fire damage dealt to this card by 2. .",
                AddSideTrigger(AddReduceDamageTrigger((DealDamageAction dd) => dd.Target == CharacterCard, (DealDamageAction dd) => 2));
                //When this card would be dealt fire damage, prevent it and add that many tokens to this card's heat pool
                AddSideTrigger(AddTrigger<DealDamageAction>((DealDamageAction dd) => IsVillainTarget(dd.DamageSource.Card) && dd.Target == CharacterCard && dd.DamageType == DamageType.Fire, CancelDamageAndGainTokens, TriggerType.CancelAction, TriggerTiming.Before));
                // "At the start of the villain turn, if there are at least 10 tokens in this card's heat pool, {TheLastMonsterHunter} flips.",
                AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker && CharacterCard.FindTokenPool("HeatPool").CurrentValue >= 10, (PhaseChangeAction pc) => FlipThisCharacterCardResponse(null), TriggerType.FlipCard));
                // "At the end of the villain turn, activate the [b]Attack[/b] text on each Weapon in play."
                AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => ActivateAllWeapons(), TriggerType.Other));
            }
            else
            {
                if (IsGameAdvanced)
                {
                    // "Increase damage dealt to hero targets by 1."
                    AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => IsHeroTarget(dd.Target), 1));
                }
                // "Increase damage dealt to villain targets by 1.",
                AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => IsVillainTarget(dd.Target), 1));
                // "At the start of the villain turn, if there are no tokens in this card's heat pool, {TheLastMonsterHunter} flips.",
                AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker && CharacterCard.FindTokenPool("HeatPool").CurrentValue == 0, (PhaseChangeAction pc) => FlipThisCharacterCardResponse(null), TriggerType.FlipCard));
                //"The first time each turn {TheLastMonsterHunter} deals each target non-fire damage, he deals that target 2 fire damage."
                AddSideTrigger(AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.DamageType != DamageType.Fire && Journal.DealDamageEntriesThisTurn().Where((DealDamageJournalEntry je) => je.DamageType != DamageType.Fire && je.SourceCard == CharacterCard && je.TargetCard == dd.Target).Count() <= 1, (DealDamageAction dd) => DealDamage(CharacterCard, dd.Target, 2, DamageType.Fire, cardSource: GetCardSource()), TriggerType.DealDamage, TriggerTiming.After));
                // "At the end of the villain turn, this card deals each target 2 fire damage, then remove 10 tokens from this card's heat pool."
                AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DealDamage(CharacterCard, (Card c) => c.IsTarget, 2, DamageType.Fire), TriggerType.DealDamage));
                AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => GameController.RemoveTokensFromPool(CharacterCard.FindTokenPool("HeatPool"), 10, cardSource: GetCardSource()), TriggerType.ModifyTokens));
            }
        }

        private IEnumerator CancelDamageAndGainTokens(DealDamageAction dd)
        {
            //TODO fix this to deal with pretend nonsense
            if (!dd.IsPretend) {
                int amount = dd.Amount;
                IEnumerator coroutine = GameController.CancelActionEx(dd, isPreventEffect: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            
                coroutine = GameController.AddTokensToPool(CharacterCard.FindTokenPool("HeatPool"), amount, cardSource: GetCardSource());
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

        private IEnumerator ActivateAllWeapons()
        {
            List<Card> weapons = FindCardsWhere((Card c) => c.DoKeywordsContain("weapon") && c.IsInPlayAndHasGameText && c.HasActivatableAbility("attack")).ToList();
            SelectCardsDecision dec = new SelectCardsDecision(GameController, DecisionMaker, (Card c) => weapons.Contains(c), SelectionType.ActivateAbility, weapons.Count(), eliminateOptions: true, allowAutoDecide: true, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectCardsAndDoAction(dec, (SelectCardDecision scd) => ActivateWeaponAttack(scd.SelectedCard), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator ActivateWeaponAttack(Card weapon)
        {
            TLMHWeaponCardController weaponCardController = FindCardController(weapon) as TLMHWeaponCardController;
            if (weapon.IsInPlay)
            {
                string key = "attack";
                IEnumerator coroutine = base.GameController.ActivateAbility(weaponCardController.GetActivatableAbilities(key).First(), GetCardSource());
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
