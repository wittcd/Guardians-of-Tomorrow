using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Argus
{

	public class ScoutAheadCardController : CardController
	{
		public ScoutAheadCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}


		public override IEnumerator Play()
		{
			//draw, reveal 2 envs, replace onto top and bottom, operate.
			IEnumerator coroutine = DrawCard(base.TurnTaker.ToHero(), false, null, true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = RevealCardsFromTopOfDeck_PutOnTopAndOnBottom(DecisionMaker, base.TurnTakerController, FindEnvironment().TurnTaker.Deck);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = GameController.SelectAndActivateAbility(DecisionMaker, "operate", new LinqCardCriteria((Card c) => c.IsInPlay, "camdrone in play"), null, false, GetCardSource());
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
