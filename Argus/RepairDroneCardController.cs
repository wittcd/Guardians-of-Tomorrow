using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Argus
{

	public class RepairDroneCardController : CamdroneCardController
	{
		public RepairDroneCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
				base.SpecialStringMaker.ShowNumberOfCardsAtLocations(() => from httc in base.GameController.FindHeroTurnTakerControllers()
																		   where !httc.IsIncapacitatedOrOutOfGame && httc.BattleZone == base.BattleZone
																		   select httc.TurnTaker.Trash, new LinqCardCriteria((Card c) => IsEquipment(c), "equipment"));
		}

		public override IEnumerator ActivateOperate()
		{
			//move an equipment card from a trash to its owner's hand
			List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
			GameController gameController = base.GameController;
			HeroTurnTakerController decisionMaker = DecisionMaker;
			Func<TurnTaker, bool> additionalCriteria = (TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && tt.Trash.Cards.Where((Card c) => IsEquipment(c)).Count() > 0;
			CardSource cardSource = GetCardSource();
			IEnumerator coroutine = gameController.SelectTurnTaker(decisionMaker, SelectionType.MoveCardToHandFromTrash, storedResults, optional: true, allowAutoDecide: false, additionalCriteria, null, null, checkExtraTurnTakersInstead: false, canBeCancelled: true, ignoreBattleZone: false, cardSource);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			SelectTurnTakerDecision selectTurnTakerDecision = storedResults.FirstOrDefault();
			if (selectTurnTakerDecision != null && selectTurnTakerDecision.SelectedTurnTaker != null)
			{
				HeroTurnTakerController heroTurnTakerController = FindHeroTurnTakerController(selectTurnTakerDecision.SelectedTurnTaker.ToHero());
				MoveCardDestination obj = new MoveCardDestination(heroTurnTakerController.HeroTurnTaker.Hand);
				coroutine = base.GameController.SelectCardFromLocationAndMoveIt(heroTurnTakerController, heroTurnTakerController.TurnTaker.Trash, new LinqCardCriteria((Card c) => IsEquipment(c), "equipment"), obj.ToEnumerable(), isPutIntoPlay: false, playIfMovingToPlayArea: true, shuffleAfterwards: false, optional: true, null, flipFaceDown: false, showOutput: false, null, isDiscardIfMovingtoTrash: false, allowAutoDecide: false, null, null, GetCardSource());
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

		public override IEnumerator UsePower(int index = 0)
		{
			int healAmount = GetPowerNumeral(0, 1);
			int numTargets = GetPowerNumeral(1, 2);
			IEnumerator coroutine = GameController.SelectAndGainHP(DecisionMaker, healAmount, false, (Card c) => c.IsHero, numTargets, 0, false, null, GetCardSource());
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
