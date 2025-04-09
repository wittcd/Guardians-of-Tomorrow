using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Argus
{

	public class RebuildAndReuseCardController : CardController
	{
		public RebuildAndReuseCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("camdrone"), "camdrone"));
		}


		public override IEnumerator Play()
		{
			LinqCardCriteria isCamdrone = new LinqCardCriteria((Card c) => c.DoKeywordsContain("camdrone"), "camdrone");
			MoveCardDestination obj = new MoveCardDestination(DecisionMaker.TurnTaker.PlayArea);
			//move up to 2 camdrones from trash to play
			List<SelectCardDecision> selectedCards = new List<SelectCardDecision>();
			IEnumerator coroutine = GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, DecisionMaker.TurnTaker.Trash, 0, 2, isCamdrone, obj.ToEnumerable(), true, true, false, false, selectedCards, null, selectionType: SelectionType.MoveCard, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//activate up to 2 Operate texts
			List<Card> usedCards = new List<Card>();
			LinqCardCriteria unused = new LinqCardCriteria((Card c) => !usedCards.Contains(c) && c.IsInPlay);
			for (int x = 0; x < 2; x++)
			{
				List<ActivateAbilityDecision> activated = new List<ActivateAbilityDecision>();
				coroutine = GameController.SelectAndActivateAbility(DecisionMaker, "operate", unused, activated, true, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				if (activated.FirstOrDefault() != null)
				{
					usedCards.Add(activated.FirstOrDefault().SelectedCard);
				}
				else
				{
					x = 2;
				}
			}

		}

	}
}
