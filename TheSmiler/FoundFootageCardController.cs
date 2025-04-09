using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheSmiler
{
    public class FoundFootageCardController : CardController
    {
        public FoundFootageCardController(Card card, TurnTakerController turnTakerController)
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
            AddEndOfTurnTrigger((TurnTaker tt) => tt == Card.Location.OwnerTurnTaker, (PhaseChangeAction pc) => OptionalDrawThenDamage(), new TriggerType[] { TriggerType.DrawCard, TriggerType.DealDamage });
        }

        private IEnumerator OptionalDrawThenDamage()
        {
            //At the end of the hero whose play area this card is in's turn, they may draw a card. 
            HeroTurnTaker CurrentHero = Card.Location.OwnerTurnTaker.ToHero();
            List<DrawCardAction> draws = new List<DrawCardAction>();
            IEnumerator coroutine = GameController.DrawCard(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => tt.ToHero() == CurrentHero), true, null, false, draws, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If they do, {TheSmiler} deals that hero 1 psychic damage.
            if (DidDrawCards(draws))
            {
                coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c == CurrentHero.CharacterCard, 1, DamageType.Psychic, cardSource: GetCardSource());
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
