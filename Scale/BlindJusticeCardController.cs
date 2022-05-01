using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class BlindJusticeCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public BlindJusticeCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//reduce damage dealt by Scale by 2 until end of your next turn
			ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(2);
			rdse.UntilEndOfNextTurn(base.TurnTaker);
			rdse.SourceCriteria.IsSpecificCard = base.CharacterCard;
			IEnumerator coroutine = AddStatusEffect(rdse);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//draw 4
			coroutine = DrawCards(DecisionMaker, 4);
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