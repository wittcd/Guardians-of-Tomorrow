using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Price
{
    class FamilyCurseCardController : CardController
    {
        public FamilyCurseCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) 
        { 
        
        }

        public override IEnumerator Play()
        {
            List<RevealCardsAction> storedResults = new List<RevealCardsAction>();
            IEnumerator coroutine = GameController.RevealCards(TurnTakerController, TurnTaker.Deck, (Card c) => true, 1, storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Card revealed = storedResults.FirstOrDefault().RevealedCards.FirstOrDefault();

            if (revealed.DoKeywordsContain("pact"))
            {
                coroutine = GameController.PlayCard(TurnTakerController, revealed, true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 5, DamageType.Infernal, 1, false, 1, cardSource: GetCardSource());
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
                coroutine = GameController.MoveCard(TurnTakerController, revealed, TurnTaker.ToHero().Hand, showMessage: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.GainHP(CharacterCard, 4, cardSource: GetCardSource());
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
