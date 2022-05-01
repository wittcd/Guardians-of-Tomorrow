using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class GraviticStrikeCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public GraviticStrikeCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//scale deals 1 target 3 melee damage and up to 2 other targets 1 energy damage each. Destroy an env card
			List<DealDamageAction> targets = new List<DealDamageAction>();
			GameController gameController = base.GameController;
			HeroTurnTakerController decisionMaker = DecisionMaker;
			DamageSource source = new DamageSource(base.GameController, base.CharacterCard);
			int? numberOfTargets = 1;
			int? requiredTargets = 1;
			List<DealDamageAction> storedResultsDamage = targets;
			CardSource cardSource = GetCardSource();
			IEnumerator coroutine = gameController.SelectTargetsAndDealDamage(decisionMaker, source, 3, DamageType.Melee, numberOfTargets, optional: false, requiredTargets, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null, null, storedResultsDamage, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, cardSource);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			GameController gameController2 = base.GameController;
			HeroTurnTakerController decisionMaker2 = DecisionMaker;
			DamageSource source2 = new DamageSource(base.GameController, base.CharacterCard);
			int? numberOfTargets2 = 2;
			int? requiredTargets2 = 0;
			Func<Card, bool> additionalCriteria = (Card card) => !targets.Select((DealDamageAction d) => d.Target).Contains(card);
			cardSource = GetCardSource();
			IEnumerator coroutine2 = gameController2.SelectTargetsAndDealDamage(decisionMaker2, source2, 1, DamageType.Energy, numberOfTargets2, optional: false, requiredTargets2, isIrreducible: false, allowAutoDecide: false, autoDecide: false, additionalCriteria, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, cardSource);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}

			coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), optional: false, null, null, GetCardSource());
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