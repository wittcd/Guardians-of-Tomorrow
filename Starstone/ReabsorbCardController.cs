using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class ReabsorbCardController : CardController
	{
		public ReabsorbCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
			IEnumerator coroutine = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.Owner == base.TurnTaker && c.DoKeywordsContain("stone", false, false)), optional: true, storedResults, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (DidDestroyCards(storedResults, 1))
			{
				GameController gameController = base.GameController;
				HeroTurnTakerController decisionMaker = DecisionMaker;
				Func<Card, bool> criteria = (Card c) => c == base.CharacterCard || c.DoKeywordsContain("stone", false, false);
				CardSource cardSource = GetCardSource();
				coroutine = gameController.GainHP(decisionMaker, criteria, 2, null, optional: false, null, null, null, cardSource);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				coroutine = DrawCards(DecisionMaker, 1);
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
