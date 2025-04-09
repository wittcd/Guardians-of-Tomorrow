using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class ShiftBalanceCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public ShiftBalanceCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//Destroy a balance card.

			LinqCardCriteria isBalance = new LinqCardCriteria((Card c) => c.DoKeywordsContain("balance"), "balance cards");
			List<DestroyCardAction> storedDestroy = new List<DestroyCardAction>();
			IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker.ToHero(), isBalance, true, storedDestroy, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//search your deck for a balance card and put it into play, then shuffle it.
			
			coroutine = base.GameController.SelectCardFromLocationAndMoveIt(base.HeroTurnTakerController, base.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("balance"), () => "balance"), new MoveCardDestination[1]
			{
				new MoveCardDestination(base.TurnTaker.PlayArea)
			}, isPutIntoPlay: true, playIfMovingToPlayArea: true, shuffleAfterwards: true, optional: false, null, flipFaceDown: false, showOutput: false, null, isDiscardIfMovingtoTrash: false, allowAutoDecide: false, null, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			
			// {Scale} deals each non-hero target 1 infernal damage

			coroutine = GameController.DealDamage(DecisionMaker.ToHero(), base.CharacterCard, (Card c) => !IsHero(c), 2, DamageType.Infernal, false, false, null, cardSource: GetCardSource());
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