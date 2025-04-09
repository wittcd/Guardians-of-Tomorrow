using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class StoneshapeCardController : CardController
	{
		public StoneshapeCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			IEnumerator coroutine = base.GameController.SelectAndReturnCards(base.HeroTurnTakerController, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("stone", false, false) && c.Owner == base.TurnTaker, "stone"), toHand: true, toDeck: false, optional: false, 0, null, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			List<PlayCardAction> storedResults = new List<PlayCardAction>();
			coroutine = SelectAndPlayCardsFromHand(base.HeroTurnTakerController, 1, optional: false, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("stone", false, false), "stone"), isPutIntoPlay: false, storedResults: storedResults);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (storedResults.FirstOrDefault() != null)
			{
				Card card = storedResults.FirstOrDefault().CardToPlay;
				if (card != null)
				{
					DamageSource ds = new DamageSource(base.GameController, card);
					coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, ds, 4, DamageType.Melee, 1, false, 1, false, false, false, null, null, null, null, null, false, null, null, false, null, GetCardSource());
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
}