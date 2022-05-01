using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Argus
{

	public class ShockTacticsCardController : CardController
	{
		public ShockTacticsCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}


		public override IEnumerator Play()
		{
			//deal 1 target 2 lightning damage. Activate 2 Operates.
			GameController gameController = base.GameController;
			HeroTurnTakerController decisionMaker = DecisionMaker;
			DamageSource source = new DamageSource(base.GameController, base.CharacterCard);
			int? numberOfTargets = 1;
			int? requiredTargets = 1;
			CardSource cardSource = GetCardSource();
			IEnumerator coroutine = gameController.SelectTargetsAndDealDamage(decisionMaker, source, 2, DamageType.Lightning, numberOfTargets, optional: false, requiredTargets, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, cardSource);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			List<ActivateAbilityDecision> storedActivate = new List<ActivateAbilityDecision>();
			coroutine = GameController.SelectAndActivateAbility(DecisionMaker, "operate", new LinqCardCriteria((Card c) => c.IsInPlay, "in play"), storedActivate, false, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = GameController.SelectAndActivateAbility(DecisionMaker, "operate", new LinqCardCriteria((Card c) => c.IsInPlay && (storedActivate.FirstOrDefault() == null || c != storedActivate.FirstOrDefault().SelectedCard), "in play"), null, false, GetCardSource());
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
