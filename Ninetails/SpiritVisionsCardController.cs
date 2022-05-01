using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class SpiritVisionsCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public SpiritVisionsCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//first damage dealt each turn, redirect to ghost in play area.
			AddFirstTimePerTurnRedirectTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && dd.Amount > 0, "RedirectOpportunityPresented", TargetType.SelectTarget, (Card c) => c.DoKeywordsContain("ghost", false, false) && c.IsAtLocationRecursive(base.CharacterCard.Location), 1, optional: false);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			//discard. If you do, put a ghost from trash into play
			List<DiscardCardAction> discards = new List<DiscardCardAction>();
			IEnumerator coroutine = base.GameController.SelectAndDiscardCard(DecisionMaker, optional: true, null, discards, SelectionType.DiscardCard, null, DecisionMaker.TurnTaker, ignoreBattleZone: false, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (discards.Count() > 0)
			{
				coroutine = base.GameController.SelectCardFromLocationAndMoveIt(base.HeroTurnTakerController, base.HeroTurnTakerController.HeroTurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("ghost", false, false), "ghost"), new MoveCardDestination[2]
			{
				new MoveCardDestination(base.HeroTurnTakerController.HeroTurnTaker.PlayArea),
				new MoveCardDestination(base.HeroTurnTakerController.HeroTurnTaker.Hand)
			}, isPutIntoPlay: true, playIfMovingToPlayArea: true, shuffleAfterwards: false, optional: false, null, flipFaceDown: false, showOutput: false, null, isDiscardIfMovingtoTrash: false, allowAutoDecide: false, null, null, GetCardSource());
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