using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Ludramor
{
    class ShareholderMeetingCardController : CardController
    {
        public ShareholderMeetingCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay("PlanetcrackerBarge");
        }

        public override IEnumerator Play()
        {
            if (FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "PlanetcrackerBarge").Count() > 0)
            {
                IEnumerator coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.IsTarget, "target", false), 1);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.GainHP(DecisionMaker, (Card c) => c.DoKeywordsContain("executive"), 5, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else {
                IEnumerator coroutine = DealDamage(CharacterCard, CharacterCard, 5, DamageType.Psychic, true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.PlayTopCardOfLocation(TurnTakerController, TurnTaker.Deck, cardSource: GetCardSource());
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
