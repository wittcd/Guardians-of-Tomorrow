using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class DrawMaterialCardController : CardController
	{
		public DrawMaterialCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			IEnumerator coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsOngoing || c.IsEnvironment, "ongoing or environment"), optional: false, null, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			MoveCardDestination obj = new MoveCardDestination(base.HeroTurnTakerController.HeroTurnTaker.Hand);
			//coroutine = base.GameController.SelectCardFromLocationAndMoveIt(base.HeroTurnTakerController, base.HeroTurnTakerController.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("stone", false, false), "stone"), obj.ToEnumerable(), isPutIntoPlay: false, playIfMovingToPlayArea: true, shuffleAfterwards: false, optional: true, null, flipFaceDown: false, showOutput: false, null, isDiscardIfMovingtoTrash: false, allowAutoDecide: false, null, null, GetCardSource());
			coroutine = base.GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, base.HeroTurnTakerController.TurnTaker.Trash, 0, 2, new LinqCardCriteria((Card c) => c.DoKeywordsContain("stone", false, false), "stone"), obj.ToEnumerable(), false, true, false, false, null, null, false, false, false, TurnTaker, false, false, null, null, GetCardSource());
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