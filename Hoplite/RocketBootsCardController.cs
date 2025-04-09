using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Hoplite
{

	public class RocketBootsCardController : CardController
	{
		public override bool AllowFastCoroutinesDuringPretend => false;


		public RocketBootsCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddTrigger((DealDamageAction dd) => dd.DidDealDamage && dd.Target == base.CharacterCard && dd.DamageSource.Card == base.CharacterCard, (DealDamageAction dd) => DrawResponse(), TriggerType.DrawCard, TriggerTiming.After);
		}

		public IEnumerator DrawResponse()
		{
			IEnumerator coroutine = base.GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public override IEnumerator UsePower(int index = 0)
        {
			int amount = GetPowerNumeral(0, 1);

			ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(amount);
			rdse.TargetCriteria.IsSpecificCard = base.CharacterCard;
			rdse.UntilStartOfNextTurn(TurnTaker);

			IEnumerator coroutine = GameController.AddStatusEffect(rdse, true, GetCardSource());
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