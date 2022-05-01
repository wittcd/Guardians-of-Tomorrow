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
			AddTrigger((DealDamageAction dd) => dd.Amount >= 4 && dd.Target == base.CharacterCard, DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.Before);
		}


	}
}