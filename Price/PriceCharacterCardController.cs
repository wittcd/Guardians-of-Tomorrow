using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Price
{
    class PriceCharacterCardController : HeroCharacterCardController
    {
        public static readonly string PreventDrawPropertyKey = "PriceCannotDraw";
        public static readonly string HasGameStartedKey = "GameHasStarted";
        public PriceCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            CannotDrawCards((TurnTakerController ttc) => ttc == TurnTakerController && GameController.GetCardPropertyJournalEntryBoolean(Card, PreventDrawPropertyKey) != null && GameController.GetCardPropertyJournalEntryBoolean(Card, PreventDrawPropertyKey).Value == true);
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => ResumeDrawEffect(), TriggerType.Other);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeralTargets = GetPowerNumeral(0, 1);
            int powerNumeralDamage = GetPowerNumeral(1, 2);
            List<Function> options = new List<Function>();
            options.Add(new Function(DecisionMaker, "Become unable to draw cards.", SelectionType.None, PreventDrawsUntilStartOfTurn));
            SelectFunctionDecision sfd = new SelectFunctionDecision(GameController, DecisionMaker, options, true, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(sfd);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), powerNumeralDamage, DamageType.Infernal, powerNumeralTargets, false, powerNumeralTargets, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator PreventDrawsUntilStartOfTurn()
        {
            GameController.AddCardPropertyJournalEntry(Card, PreventDrawPropertyKey, true);
            OnPhaseChangeStatusEffect effect = new OnPhaseChangeStatusEffect(base.CardWithoutReplacements, "DoNothing", TurnTaker.Name + " cannot draw cards.", new TriggerType[1] { TriggerType.CreateStatusEffect }, base.Card);
            effect.UntilStartOfNextTurn(base.TurnTaker);
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
        public IEnumerator ResumeDrawEffect()
        {
            GameController.AddCardPropertyJournalEntry(Card, PreventDrawPropertyKey, false);
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        IEnumerator coroutine = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
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
                        IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "Environment"), false, cardSource: GetCardSource());
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
                        List<SelectLocationDecision> selectedDeck = new List<SelectLocationDecision>();
                        IEnumerator coroutine = GameController.SelectADeck(DecisionMaker, SelectionType.DiscardFromDeck, (Location l) => l.IsDeck, selectedDeck, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        Location loc = selectedDeck.FirstOrDefault().SelectedLocation.Location;
                        coroutine = GameController.MoveCard(TurnTakerController, loc.TopCard, loc.OwnerTurnTaker.Trash, isDiscard: true, cardSource: GetCardSource());
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
