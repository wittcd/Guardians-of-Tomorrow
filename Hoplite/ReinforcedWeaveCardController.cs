using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Hoplite
{

	public class ReinforcedWeaveCardController : CardController
	{

		public ReinforcedWeaveCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageType == DamageType.Melee || dd.DamageType == DamageType.Projectile, 1, null, (Card c) => c == base.CharacterCard);
			AddTrigger((DealDamageAction dd) => dd.Amount >= 4 && dd.Target == base.CharacterCard, (DealDamageAction dd) => PreventAndDestroyResponse(dd), TriggerType.DestroySelf, TriggerTiming.Before);
		}

		private IEnumerator PreventAndDestroyResponse(DealDamageAction dd)
        {
			IEnumerator coroutine = GameController.CancelAction(dd, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = GameController.DestroyCard(DecisionMaker, base.Card, cardSource: GetCardSource());
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