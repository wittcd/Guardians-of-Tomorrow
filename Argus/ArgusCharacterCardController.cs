using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Argus
{
    public class ArgusCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public ArgusCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Draw 1 card
            var numberOfCards = GetPowerNumeral(0, 1);
            IEnumerator e = DrawCards(this.HeroTurnTakerController, numberOfCards);

            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }

            //operate
            LinqCardCriteria isCamdrone = new LinqCardCriteria((Card c) => c.DoKeywordsContain("camdrone") && c.IsInPlay, "camdrone");
            IEnumerator coroutine = GameController.SelectAndActivateAbility(DecisionMaker, "operate", isCamdrone, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        // One player may draw a card now.
                        GameController gameController2 = base.GameController;
                        HeroTurnTakerController decisionMaker = DecisionMaker;
                        CardSource cardSource = GetCardSource();
                        IEnumerator coroutine3 = gameController2.SelectHeroToDrawCard(decisionMaker, optionalSelectHero: false, optionalDrawCard: true, allowAutoDecideHero: false, null, null, null, cardSource);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
                case 1:
                    {
                        // One hero may play a card now.
                        IEnumerator coroutine2 = SelectHeroToPlayCard(DecisionMaker);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        // reveal and replace the top card of the environment deck
                        List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                        Location deck = FindEnvironment().TurnTaker.Deck;

                        // reveal top card
                        List<Card> storedResultsCard = new List<Card>();
                        IEnumerator coroutine = base.GameController.RevealCards(base.TurnTakerController, deck, 1, storedResultsCard, fromBottom: false, RevealedCardDisplay.ShowRevealedCards, null, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        Card card = storedResultsCard.FirstOrDefault();
                        if (card != null)
                        {
                            coroutine = base.GameController.MoveCard(base.TurnTakerController, card, deck, showMessage: true, cardSource: GetCardSource());
                        }
                        List<Location> list2 = new List<Location>();
			            list2.Add(deck.OwnerTurnTaker.Revealed);
			            coroutine = CleanupCardsAtLocations(list2, deck, toBottom: false, addInhibitorException: true, shuffleAfterwards: false, sendMessage: false, isDiscard: false, isReturnedToOriginalLocation: true, storedResultsCard);
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