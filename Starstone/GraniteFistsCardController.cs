using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class GraniteFistsCardController : CardController
	{
		public GraniteFistsCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator UsePower(int index = 0)
		{
			int powerNumeralTargets = GetPowerNumeral(0, 2);
			int powerNumeralDamage = GetPowerNumeral(1, 2);
			int powerNumeralDraw = GetPowerNumeral(2, 2);
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), powerNumeralDamage, DamageType.Melee, powerNumeralTargets, optional: false, 0, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//you may destroy a stone. if you do, draw 2 cards.
			List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
			coroutine = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.Owner == base.TurnTaker && c.DoKeywordsContain("stone", false, false)), optional: true, storedResults, null, GetCardSource());
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
				coroutine = DrawCards(DecisionMaker, powerNumeralDraw);
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
