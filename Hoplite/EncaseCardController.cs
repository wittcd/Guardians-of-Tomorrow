using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Hoplite
{
    class EncaseCardController : CardController
    {
        public EncaseCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        //{Hoplite} deals 1 target 4 energy damage.
        //Search your deck or trash for a copy of Shield Bubble
        //and put it into play. If you searched your deck, shuffle it.

        public override IEnumerator Play()
        {
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), 4, DamageType.Energy, 1, false, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

			if (base.TurnTaker.IsHero)
			{
				Card card = FindCard("ShieldBubble");
				if (card != null)
				{
					coroutine = SearchForCards(DecisionMaker, searchDeck: true, searchTrash: true, 1, 1, new LinqCardCriteria((Card c) => c.Identifier == "ShieldBubble", "named Shield Bubble", useCardsSuffix: false, useCardsPrefix: true), putIntoPlay: true, putInHand: false, putOnDeck: false, optional: false, null, autoDecideCard: false, false);
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
					coroutine = base.GameController.SendMessageAction("Shield Bubble is not in the game.", Priority.Low, GetCardSource(), null, showCardSource: true);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
				}
				coroutine = ShuffleDeck(DecisionMaker, base.TurnTaker.Deck);
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
				IEnumerator coroutine2 = base.GameController.SendMessageAction(base.Card.Title + " has no deck to search.", Priority.Medium, GetCardSource(), null, showCardSource: true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
		}
    }
}
