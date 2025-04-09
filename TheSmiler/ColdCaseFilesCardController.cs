using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheSmiler
{
    public class ColdCaseFilesCardController : CardController
    {
        public ColdCaseFilesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            List<SelectTurnTakerDecision> turntakers = new List<SelectTurnTakerDecision>();
            GameController gameController = base.GameController;
            HeroTurnTakerController decisionMaker = DecisionMaker;
            Func<TurnTaker, bool> additionalCriteria = (TurnTaker tt) => IsHero(tt);
            CardSource cardSource = GetCardSource();
            IEnumerator coroutine = gameController.SelectTurnTaker(decisionMaker, SelectionType.MoveCardToPlayArea, turntakers, optional: false, allowAutoDecide: false, additionalCriteria, null, null, checkExtraTurnTakersInstead: false, canBeCancelled: true, ignoreBattleZone: false, cardSource);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            SelectTurnTakerDecision selectTurnTakerDecision = turntakers.FirstOrDefault();
            if (selectTurnTakerDecision != null && selectTurnTakerDecision.SelectedTurnTaker != null && turntakers != null)
            {
                storedResults.Add(new MoveCardDestination(selectTurnTakerDecision.SelectedTurnTaker.PlayArea, toBottom: false, showMessage: true));
                yield break;
            }
        }

        public override void AddTriggers()
        {
            // Reduce damage dealt by targets in this play area by 1. At the start of the environment turn, one hero in this play area either draws a card or gains 1 HP.
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card.Location.HighestRecursiveLocation == Card.Location.HighestRecursiveLocation, (DealDamageAction dd) => 1);
            AddStartOfTurnTrigger((TurnTaker tt) => tt.IsEnvironment, (PhaseChangeAction pc) => HeroInThisPlayAreaDrawsOrHeals(), new TriggerType[] { TriggerType.DrawCard, TriggerType.GainHP });
        }

        private IEnumerator HeroInThisPlayAreaDrawsOrHeals()
        {
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectHeroCharacterCard(DecisionMaker, SelectionType.MakeDecision, storedResults, false, false, GetCardSource(), (Card c) => c.Location.HighestRecursiveLocation == Card.Location.HighestRecursiveLocation);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectCard(storedResults))
            {
                Card selectedHero = storedResults.FirstOrDefault().SelectedCard;

                List<Function> list = new List<Function>();
                Function draw = new Function(DecisionMaker, "Draw a card", SelectionType.DrawCard, () => GameController.DrawCard(selectedHero.Owner.ToHero(), false, cardSource: GetCardSource()));
                Function heal = new Function(DecisionMaker, "Gain 1 HP", SelectionType.GainHP, () => GameController.GainHP(selectedHero, 1, cardSource: GetCardSource()));
                list.Add(draw);
                list.Add(heal);
                SelectFunctionDecision sf = new SelectFunctionDecision(GameController, DecisionMaker, list, false, cardSource: GetCardSource());
                coroutine = base.GameController.SelectAndPerformFunction(sf);
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
