using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class AChangeOfFaceCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public AChangeOfFaceCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => !dd.Target.IsHero && !dd.DamageSource.IsHero, 1));
		}

		public override IEnumerator UsePower(int index = 0)
		{
			List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
			IEnumerator coroutine = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlay && (c.IsOngoing), "ongoing"), optional: true, storedResults, base.Card, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (storedResults.Count() > 0 && storedResults.Any((DestroyCardAction dc) => dc.CardToDestroy.Card.Owner == base.TurnTaker))
			{
				coroutine = SelectAndPlayCardFromHand(DecisionMaker);
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
				coroutine = SelectAndDiscardCards(DecisionMaker, 1, false, null, null, false, null, null, null, SelectionType.DiscardCard, base.TurnTaker);
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