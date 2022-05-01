using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class CometCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public CometCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddReduceDamageTrigger((Card c) => true, 1);
		}

	}
}