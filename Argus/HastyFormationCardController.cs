using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace GuardiansOfTomorrow.Argus
{
    class HastyFormationCardController : CardController
    {
        public HastyFormationCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = GameController.DrawCards(DecisionMaker, 2, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            LinqCardCriteria isCam = new LinqCardCriteria((Card c) => c.DoKeywordsContain("camdrone"), "camdrone");
            coroutine = GameController.SelectAndPlayCardsFromHand(DecisionMaker, 2, false, 0, isCam, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            
            List<Card> usedCards = new List<Card>();
            LinqCardCriteria unused = new LinqCardCriteria((Card c) => !usedCards.Contains(c) && c.IsInPlay);
            for (int x = 0; x < 2; x++) 
            {
                List<ActivateAbilityDecision> activated = new List<ActivateAbilityDecision>();
                coroutine = GameController.SelectAndActivateAbility(DecisionMaker, "operate", unused, activated, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (activated.FirstOrDefault() != null)
                {
                    usedCards.Add(activated.FirstOrDefault().SelectedCard);
                }
                else
                {
                    x = 2;
                }
            }
        }
    }
}
