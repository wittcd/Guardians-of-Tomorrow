using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class BurningWhisperCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public BurningWhisperCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//increase fire damage by 1
			AddIncreaseDamageTrigger((DealDamageAction dealDamage) => dealDamage.DamageType == DamageType.Fire, 1);
			//start of turn destroy self
			AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
		}

		public override IEnumerator Play()
		{
			//Ninetails deals up to 2 targets 1 fire damage each
			DamageSource source2 = new DamageSource(base.GameController, base.CharacterCard);
			int? numberOfTargets2 = 2;
			int? requiredTargets2 = 0;
			//Func<Card, bool> additionalCriteria = (Card card) => !targets.Select((DealDamageAction d) => d.Target).Contains(card);
			CardSource cardSource = GetCardSource();
			IEnumerator coroutine2 = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, source2, 1, DamageType.Fire, numberOfTargets2, optional: false, requiredTargets2, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null /*additionalCriteria*/, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
		}
	}
}