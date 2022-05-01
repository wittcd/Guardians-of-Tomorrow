using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class IgneousShieldCardController : CardController
	{
		public IgneousShieldCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}
		
		public override void AddTriggers()
		{
			AddRedirectDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target.IsHero, () => base.Card, optional: true);
		}

	}
}
