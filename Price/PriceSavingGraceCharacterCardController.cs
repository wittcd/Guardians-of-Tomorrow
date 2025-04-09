using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Price
{
    class PriceSavingGraceCharacterCardController : HeroCharacterCardController
    {
        public static readonly string HealOnInfernalPropertyKey = "PriceHealsOnInfernalDamage";
        public PriceSavingGraceCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            GameController.AddCardPropertyJournalEntry(Card, HealOnInfernalPropertyKey, false); 
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => RemoveHealEffect(pc), TriggerType.Other);
            AddPreventDamageTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && dd.DamageType == DamageType.Infernal && GameController.GetCardPropertyJournalEntryBoolean(Card, HealOnInfernalPropertyKey).HasValue && GameController.GetCardPropertyJournalEntryBoolean(Card, HealOnInfernalPropertyKey).Value, (DealDamageAction dd) => base.GameController.GainHP(base.CharacterCard, dd.Amount, null, null, GetCardSource()), new TriggerType[1] { TriggerType.GainHP }, isPreventEffect: true);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeralDraws = GetPowerNumeral(0, 2);
            int powerNumeralDiscards = GetPowerNumeral(1, 1);

            IEnumerator coroutine = HealOnInfernalUntilEndOfTurn();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }


            List<SelectCardDecision> moves = new List<SelectCardDecision>();
            coroutine = GameController.MoveCards(DecisionMaker, new LinqCardCriteria((Card c) => c.DoKeywordsContain("pact") && c.Location == TurnTaker.Trash, "pact"), (Card c) => TurnTaker.Deck, optional: true, storedResults: moves, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidSelectCards(moves))
            {
                coroutine = GameController.ShuffleLocation(TurnTaker.Deck, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            List<SelectNumberDecision> numbers = new List<SelectNumberDecision>();
            List<DrawCardAction> draws = new List<DrawCardAction>();
            coroutine = GameController.SelectNumberEx(DecisionMaker, SelectionType.DrawCard, 0, powerNumeralDraws, numbers, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.DrawCardsEx(new LinqTurnTakerCriteria((TurnTaker tt) => tt == TurnTaker), numbers.FirstOrDefault().SelectedNumber.Value, storedResults: draws, cardSource: GetCardSource()); ;
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidDrawCards(draws))
            {
                coroutine = GameController.SelectAndDiscardCards(DecisionMaker, GetNumberOfCardsDrawn(draws) + powerNumeralDiscards, false, GetNumberOfCardsDrawn(draws) + powerNumeralDiscards, cardSource: GetCardSource());
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

        private IEnumerator HealOnInfernalUntilEndOfTurn()
        {
            GameController.AddCardPropertyJournalEntry(Card, HealOnInfernalPropertyKey, true);
            OnPhaseChangeStatusEffect effect = new OnPhaseChangeStatusEffect(base.CardWithoutReplacements, "DoNothing", "When " + TurnTaker.Name + " would take infernal damage, she gains that much HP instead.", new TriggerType[1] { TriggerType.CreateStatusEffect }, base.Card);
            effect.UntilEndOfTurn(base.TurnTaker);
            effect.TurnTakerCriteria.IsSpecificTurnTaker = base.TurnTaker;
            effect.TurnPhaseCriteria.Phase = Phase.Start;
            effect.CanEffectStack = false;
            IEnumerator coroutine = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator RemoveHealEffect(PhaseChangeAction pc)
        {
            GameController.AddCardPropertyJournalEntry(Card, HealOnInfernalPropertyKey, false);
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        IEnumerator coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c), "Ongoing"), false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 2:
                    {
                        IEnumerator coroutine = GameController.SelectHeroToSelectTargetAndDealDamage(DecisionMaker, 1, DamageType.Infernal, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
            }
        }
    }
}
