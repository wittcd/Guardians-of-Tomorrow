using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class MeteorCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public MeteorCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageToAllResponse, TriggerType.DealDamage, null, false);
		}


		private IEnumerator DealDamageToAllResponse(PhaseChangeAction pc)
		{
			IEnumerator coroutine = DealDamage(base.CharacterCard, (Card c) => true, 2, DamageType.Energy);
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