using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class ShatterCardController : CardController
	{
		public ShatterCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
			IEnumerator coroutine = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.Owner == base.TurnTaker && c.DoKeywordsContain("stone", false, false)), optional: true, storedResults, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (DidDestroyCards(storedResults, 1))
			{
				GameController gameController = base.GameController;
				HeroTurnTakerController decisionMaker = DecisionMaker;
				DamageSource source = new DamageSource(base.GameController, base.CharacterCard);
				int? numberOfTargets = 4;
				int? requiredTargets = 0;
				CardSource cardSource = GetCardSource();
				coroutine = gameController.SelectTargetsAndDealDamage(decisionMaker, source, 2, DamageType.Projectile, numberOfTargets, optional: false, requiredTargets, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, cardSource);
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