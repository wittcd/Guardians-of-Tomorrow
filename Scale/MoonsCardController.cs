using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class MoonsCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public MoonsCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddTrigger((DealDamageAction dd) => dd.Target.IsHero, HeroRedirectResponse, new TriggerType[1] { TriggerType.RedirectDamage }, TriggerTiming.Before);
			AddTrigger((DealDamageAction dd) => dd.Target.IsVillain, VillainRedirectResponse, new TriggerType[1] { TriggerType.RedirectDamage }, TriggerTiming.Before);
			AddTrigger((DealDamageAction dd) => dd.Target.IsEnvironment, EnvironmentRedirectResponse, new TriggerType[1] { TriggerType.RedirectDamage }, TriggerTiming.Before);
		}

		private IEnumerator HeroRedirectResponse(DealDamageAction dd) 
		{
			if (!dd.IsRedirectable)
			{
				yield break;
			}
			List<Card> storedResults = new List<Card>();
			IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => c.IsHero, storedResults, null, null, evenIfCannotDealDamage: false, optional: false, null, ignoreBattleZone: false, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			Card card = storedResults.FirstOrDefault();
			if (card != null && dd.Target != card)
			{
				coroutine = base.GameController.RedirectDamage(dd, card, isOptional: false, GetCardSource());
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

		private IEnumerator VillainRedirectResponse(DealDamageAction dd)
		{
			if (!dd.IsRedirectable)
			{
				yield break;
			}
			List<Card> storedResults = new List<Card>();
			IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => c.IsVillain, storedResults, null, null, evenIfCannotDealDamage: false, optional: false, null, ignoreBattleZone: false, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			Card card = storedResults.FirstOrDefault();
			if (card != null && dd.Target != card)
			{
				coroutine = base.GameController.RedirectDamage(dd, card, isOptional: false, GetCardSource());
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

		private IEnumerator EnvironmentRedirectResponse(DealDamageAction dd)
		{
			if (!dd.IsRedirectable)
			{
				yield break;
			}
			List<Card> storedResults = new List<Card>();
			IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => c.IsEnvironment, storedResults, null, null, evenIfCannotDealDamage: false, optional: false, null, ignoreBattleZone: false, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			Card card = storedResults.FirstOrDefault();
			if (card != null && dd.Target != card)
			{
				coroutine = base.GameController.RedirectDamage(dd, card, isOptional: false, GetCardSource());
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