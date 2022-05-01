using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class JadesCollarCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public JadesCollarCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && dd.DamageSource.IsEnvironmentCard, DiscardToRedirectDamageResponse, new TriggerType[2]
			{
				TriggerType.DiscardCard,
				TriggerType.RedirectDamage
			}, TriggerTiming.Before);
		}

		private IEnumerator DiscardToRedirectDamageResponse(DealDamageAction dd)
		{
			List<DealDamageAction> damageInfo = new List<DealDamageAction> { dd };
			List<DiscardCardAction> discards = new List<DiscardCardAction>();
			IEnumerator coroutine = base.GameController.SelectAndDiscardCard(DecisionMaker, optional: true, null, discards, SelectionType.DiscardCard, damageInfo, DecisionMaker.TurnTaker, ignoreBattleZone: false, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (discards.Count() > 0)
			{
				List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
				coroutine = base.GameController.SelectTargetAndRedirectDamage(base.HeroTurnTakerController, (Card c) => c != dd.Target && IsVillainTarget(c) && c.IsInPlayAndNotUnderCard, dd, optional: false, storedResults, GetCardSource());
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