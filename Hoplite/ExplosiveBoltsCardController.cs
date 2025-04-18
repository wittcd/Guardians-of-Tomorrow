﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Hoplite
{
	public class ExplosiveBoltsCardController : CardController
	{

		public ExplosiveBoltsCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		//{Hoplite} deals up to 5 targets 2 fire damage each,
		//then deals himself fire damage equal to the number of targets
		//dealt damage this way minus 1.
		public override IEnumerator UsePower(int index = 0)
		{
			int numTargets = GetPowerNumeral(0, 5);
			int damage = GetPowerNumeral(1, 2);
			int reduction = GetPowerNumeral(2, 1);

			List<DealDamageAction> damages = new List<DealDamageAction>();
			IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), damage, DamageType.Fire, numTargets, false, 1, storedResultsDamage: damages, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (DidDealDamage(damages))
			{
				int totalTargets = 0;

				List<Card> targets = new List<Card>();

				foreach (DealDamageAction dd in damages)
				{
					if (!targets.Contains(dd.Target) && dd.DidDealDamage)
					{
						totalTargets++;
						targets.Add(dd.Target);
					}
				}
				totalTargets -= reduction;
				if (totalTargets > 0) {
					coroutine = DealDamage(base.CharacterCard, base.CharacterCard, totalTargets, DamageType.Fire, cardSource: GetCardSource());
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
}
