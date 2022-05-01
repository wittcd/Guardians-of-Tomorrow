using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Hoplite
{

	public class ShieldBubbleCardController : CardController
	{

		public ShieldBubbleCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
		{
			LinqCardCriteria validTargets = new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay, "target");
			IEnumerator coroutine = SelectCardThisCardWillMoveNextTo(validTargets, storedResults, isPutIntoPlay, decisionSources);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public override void AddTriggers()
        {
			AddTrigger((DealDamageAction dd) => dd.Target == GetCardThisCardIsNextTo() || dd.DamageSource.Card == GetCardThisCardIsNextTo(), (DealDamageAction dd) => PreventAndDestroyResponse(dd), TriggerType.DestroySelf, TriggerTiming.Before);
			AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();
		}

		private IEnumerator PreventAndDestroyResponse(DealDamageAction dd)
        {
			IEnumerator coroutine = CancelAction(dd);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (IsRealAction(dd))
            {
				coroutine = GameController.DestroyCard(base.DecisionMaker, this.Card, false, cardSource: GetCardSource());
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