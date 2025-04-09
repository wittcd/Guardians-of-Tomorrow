using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheSmiler
{
    public class FrameJobCardController : CardController
    {
        public FrameJobCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //When this card enters play, put it in the play area of the hero with the most non-character cards in their play area
            List<TurnTaker> mostCards = new List<TurnTaker>();
            IEnumerator coroutine = FindHeroWithMostCardsInPlayArea(mostCards, cardCriteria: new LinqCardCriteria((Card c) => !c.IsCharacter, "non-character"));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            LinqCardCriteria mostHero = new LinqCardCriteria((Card c) => IsHeroCharacterCard(c) && mostCards.Contains(c.Owner));
            coroutine = SelectCardThisCardWillMoveNextTo(mostHero, storedResults, isPutIntoPlay, decisionSources);
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
            AddCannotDealDamageTrigger((Card c) => c == GetCardThisCardIsNextTo());
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DestroyCardsInThisPlayAreaThenDestroyThis(), TriggerType.DestroyCard);
        }

        private IEnumerator DestroyCardsInThisPlayAreaThenDestroyThis()
        {
            IEnumerator coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => !c.IsCharacter && c.IsAtLocationRecursive(GetCardThisCardIsNextTo().Owner.PlayArea) && c != Card), H - 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DestroyThisCardResponse(null);
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
