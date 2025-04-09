using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class EmptyPowerCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public EmptyPowerCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//destroy all balance = X
			//charactercard deals 1 target X irreducible energy damage

			LinqCardCriteria isBalance = new LinqCardCriteria((Card c) => c.DoKeywordsContain("balance"));
			List<DestroyCardAction> dclist = new List<DestroyCardAction>();
			IEnumerator coroutine = DestroyCardsAndDoActionBasedOnNumberOfCardsDestroyed(DecisionMaker, isBalance, DealXDamage, true, dclist);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if (!DidDestroyCards(dclist))
            {
				coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(DecisionMaker, TurnTaker.Deck, false, false, true, isBalance, 2, null);
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
				coroutine = DoNothing();
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


		private IEnumerator DealXDamage(int d) 
		{
			IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), d * 3, DamageType.Energy, 1, false, 1, true, cardSource: GetCardSource());
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