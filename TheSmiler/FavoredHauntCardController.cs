using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheSmiler
{
    public class FavoredHauntCardController : CardController
    {
        public FavoredHauntCardController(Card card, TurnTakerController turnTakerController)
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
            //Redirect damage dealt by environment cards to {TheSmiler} to a target in this play area.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == CharacterCard && dd.DamageSource.Card.IsEnvironment, (DealDamageAction dd) => GameController.SelectTargetAndRedirectDamage(DecisionMaker, (Card c) => IsHeroTarget(c) && c.Location == Card.Location, dd, cardSource: GetCardSource()), TriggerType.RedirectDamage, TriggerTiming.Before);
            //When a non-target environment card enters play, the hero whose play area this card is in may draw a card.
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cep) => cep.CardEnteringPlay.IsEnvironment && !cep.CardEnteringPlay.IsTarget, (CardEntersPlayAction cep) => GameController.DrawCard(Card.Location.OwnerTurnTaker.ToHero(), cardSource: GetCardSource()), TriggerType.DrawCard, TriggerTiming.After);
        }

        
    }
}
