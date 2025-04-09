using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.Ludramor
{
    class ProspectingInitiativeCardController : CardController
    {
        public ProspectingInitiativeCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("analyst"), "analyst"));
            SpecialStringMaker.ShowIfSpecificCardIsInPlay("IntelligenceOfficerSalguul");
        }

        private int numAnalyst()
        {
            return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("analyst")).Count();
        }

        public override IEnumerator Play()
        {
            //check if Salguul in play
            if (FindCardsWhere((Card c) => c.Identifier == "IntelligenceOfficerSalguul" && c.IsInPlay).Count() > 0)
            {
                int count = numAnalyst();
                for (int x = 0; x < count; x++)
                {
                    IEnumerator coroutine = GameController.PlayTopCardOfLocation(FindEnvironment(), FindEnvironment().TurnTaker.Deck, cardSource: GetCardSource());
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
            else
            {
                IEnumerator coroutine;
                List<Card> salguulLocation = (List<Card>)FindCardsWhere((Card c) => c.Identifier == "IntelligenceOfficerSalguul" && (c.Location == TurnTaker.Trash || c.Location == TurnTaker.Deck));
                if (salguulLocation.Count() > 0)
                {
                    coroutine = GameController.PlayCard(TurnTakerController, salguulLocation.FirstOrDefault(), true, cardSource: GetCardSource());
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
