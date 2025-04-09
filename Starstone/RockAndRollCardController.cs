using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class RockAndRollCardController : CardController
	{
		public RockAndRollCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}


		public override IEnumerator Play()
		{
			IEnumerator coroutine = DealDamage(base.CharacterCard, (Card card) => !IsHero(card), 1, DamageType.Melee);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			MoveCardDestination obj = new MoveCardDestination(TurnTaker.PlayArea);
			coroutine = base.GameController.SelectCardFromLocationAndMoveIt(base.HeroTurnTakerController, base.HeroTurnTakerController.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("stone", false, false), "stone"), obj.ToEnumerable(), isPutIntoPlay: true, playIfMovingToPlayArea: true, shuffleAfterwards: false, optional: true, null, flipFaceDown: false, showOutput: false, null, isDiscardIfMovingtoTrash: false, allowAutoDecide: false, null, null, GetCardSource());
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
