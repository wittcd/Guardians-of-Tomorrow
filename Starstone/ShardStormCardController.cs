using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class ShardStormCardController : CardController
	{
		public ShardStormCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}


		public override IEnumerator UsePower(int index = 0)
		{
			List<DealDamageAction> list = new List<DealDamageAction>();
			int powerNumeralTargets = GetPowerNumeral(0, 1);
			int powerNumeralDamage = GetPowerNumeral(1, 1);
			int powerNumeralTimes = GetPowerNumeral(2, 3);
			for (int i = 0; i < powerNumeralTimes; i++)
			{
				list.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.Card), null, powerNumeralDamage, DamageType.Projectile));
			}
			if (list.Count() > 0)
			{
				IEnumerator coroutine = SelectTargetsAndDealMultipleInstancesOfDamage(list, null, null, powerNumeralTargets, powerNumeralTargets);
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