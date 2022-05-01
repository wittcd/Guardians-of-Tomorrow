using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class ParticleCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		private ITrigger _modifyDamageTriggerH;
		private ITrigger _modifyDamageTriggerV;
		private ITrigger _modifyDamageTriggerE;

		public override bool AllowFastCoroutinesDuringPretend => IsLowestHitPointsUnique((Card card) => card.IsHero) && IsLowestHitPointsUnique((Card card) => card.IsVillain) && IsLowestHitPointsUnique((Card card) => card.IsEnvironment);

		public ParticleCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public bool? PerformIncrease { get; set; }

		public override void AddTriggers()
		{
			_modifyDamageTriggerH = AddTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource.IsHero, IncreaseDamageIfLowestHero, TriggerType.ModifyDamageAmount, TriggerTiming.Before);
			_modifyDamageTriggerV = AddTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource.IsVillain, IncreaseDamageIfLowestVillain, TriggerType.ModifyDamageAmount, TriggerTiming.Before);
			_modifyDamageTriggerE = AddTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource.IsEnvironmentCard, IncreaseDamageIfLowestEnvironment, TriggerType.ModifyDamageAmount, TriggerTiming.Before);
		}

		private IEnumerator IncreaseDamageIfLowestHero(DealDamageAction dd)
		{
			if (base.GameController.PretendMode)
			{
				List<bool> storedResults = new List<bool>();
				IEnumerator coroutine = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dd.DamageSource.Card, highest: false, (Card card) => card.IsHero, dd, storedResults);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				if (storedResults.Count() > 0)
				{
					PerformIncrease = storedResults.First();
				}
				else
				{
					PerformIncrease = null;
				}
			}
			if (PerformIncrease.HasValue && PerformIncrease.Value)
			{
				IEnumerator coroutine2 = base.GameController.IncreaseDamage(dd, 2, isNemesisEffect: false, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			if (!base.GameController.PretendMode)
			{
				PerformIncrease = null;
			}
		}

		private IEnumerator IncreaseDamageIfLowestVillain(DealDamageAction dd)
		{
			if (base.GameController.PretendMode)
			{
				List<bool> storedResults = new List<bool>();
				IEnumerator coroutine = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dd.DamageSource.Card, highest: false, (Card card) => card.IsVillain, dd, storedResults);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				if (storedResults.Count() > 0)
				{
					PerformIncrease = storedResults.First();
				}
				else
				{
					PerformIncrease = null;
				}
			}
			if (PerformIncrease.HasValue && PerformIncrease.Value)
			{
				IEnumerator coroutine2 = base.GameController.IncreaseDamage(dd, 2, isNemesisEffect: false, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			if (!base.GameController.PretendMode)
			{
				PerformIncrease = null;
			}
		}

		private IEnumerator IncreaseDamageIfLowestEnvironment(DealDamageAction dd)
		{
			if (base.GameController.PretendMode)
			{
				List<bool> storedResults = new List<bool>();
				IEnumerator coroutine = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dd.DamageSource.Card, highest: false, (Card card) => card.IsEnvironment, dd, storedResults);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				if (storedResults.Count() > 0)
				{
					PerformIncrease = storedResults.First();
				}
				else
				{
					PerformIncrease = null;
				}
			}
			if (PerformIncrease.HasValue && PerformIncrease.Value)
			{
				IEnumerator coroutine2 = base.GameController.IncreaseDamage(dd, 2, isNemesisEffect: false, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			if (!base.GameController.PretendMode)
			{
				PerformIncrease = null;
			}
		}
	}
}