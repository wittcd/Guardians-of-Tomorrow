using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Ludramor
{
    class RushRepairsCardController : CardController
    {
        public RushRepairsCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("technician"), "technician"));
            SpecialStringMaker.ShowIfSpecificCardIsInPlay("ChiefEngineerKomac");
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("spaceship"), "spaceship"));
        }

        private int numTechnicians()
        {
            return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("technician")).Count();
        }

        public override IEnumerator Play()
        {
            //check if Komac in play
            if (FindCardsWhere((Card c) => c.Identifier == "ChiefEngineerKomac" && c.IsInPlay).Count() > 0)
            {
                IEnumerator coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, TurnTaker.Deck, true, true, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("spaceship"), "spaceship"), numTechnicians());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                IEnumerator coroutine;
                List<Card> komacLocation = (List<Card>)FindCardsWhere((Card c) => c.Identifier == "ChiefEngineerKomac" && (c.Location == TurnTaker.Trash || c.Location == TurnTaker.Deck));
                if (komacLocation.Count() > 0)
                {
                    coroutine = GameController.PlayCard(TurnTakerController, komacLocation.FirstOrDefault(), true, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
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
        }
    }
}
