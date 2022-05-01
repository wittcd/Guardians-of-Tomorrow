using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace GuardiansOfTomorrow.Argus
{
    class OcculusDroneCardController : CamdroneCardController
	{
		public OcculusDroneCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator UsePower(int index = 0)
		{
			//Reveal the top 2 cards of a deck. Put one on top of that deck and discard the other.

			int numberOfReveals = GetPowerNumeral(0, 2);

			List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
			IEnumerator coroutine = base.GameController.SelectADeck(DecisionMaker, SelectionType.RevealCardsFromDeck, (Location l) => true, storedResults, optional: false, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (DidSelectDeck(storedResults))
			{
				Location deck = storedResults.First().SelectedLocation.Location;
				List<Card> revealedCards = new List<Card>();
				coroutine = RevealCardsFromTopOfDeck_PutOnTopAndOnBottom(base.HeroTurnTakerController, base.TurnTakerController, deck, numberOfReveals, 1, 1, revealedCards);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				List<Location> list = new List<Location>();
				list.Add(deck.OwnerTurnTaker.Revealed);
				IEnumerator coroutine2 = CleanupCardsAtLocations(list, deck, toBottom: false, addInhibitorException: true, shuffleAfterwards: false, sendMessage: false, isDiscard: false, isReturnedToOriginalLocation: true, revealedCards);
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

		public override IEnumerator ActivateOperate()
		{
			IEnumerator coroutine = base.GameController.SelectHeroToDrawCard(DecisionMaker, optionalSelectHero: false, optionalDrawCard: true, allowAutoDecideHero: false, null, null, null, GetCardSource());
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
