using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class StarCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public StarCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddMakeDamageIrreducibleTrigger(((DealDamageAction dd) => true));
		}

	}
}