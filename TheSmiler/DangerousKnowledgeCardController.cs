using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheSmiler
{
    public class DangerousKnowledgeCardController : CardController
    {
        public DangerousKnowledgeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //Destroy any number of Clues.
            IEnumerator coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsClue && c.IsInPlay, "clue"), FindCardsWhere((Card c) => c.IsClue && c.IsInPlay).Count(), false, 0, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{TheSmiler} deals each hero infernal damage equal to the number of Clues in their play area.
            List<DealDamageAction> storedDamage = new List<DealDamageAction>();
            if (FindCardsWhere((Card c) => c.IsClue && c.IsInPlay).Count() > 0)
            {
                coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => IsHeroCharacterCard(c), FindCardsWhere((Card c) => c.IsClue && c.IsInPlay).Count(), DamageType.Infernal, storedResults: storedDamage, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //If no damage is dealt this way, play the top card of the villain deck.
            if (!DidDealDamage(storedDamage))
            {
                coroutine = GameController.PlayTopCard(DecisionMaker, TurnTakerController, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            
            //Shuffle the villain trash into the villain deck.
            coroutine = GameController.ShuffleTrashIntoDeck(TurnTakerController, cardSource: GetCardSource());
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
