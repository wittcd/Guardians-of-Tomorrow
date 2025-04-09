using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Hoplite
{
	public class BreachingChargeCardController : CardController
	{

		public BreachingChargeCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		//Destroy a non-target environment card. If no card is destroyed this way,
		//{Hoplite} deals himself and each non-hero target 2 fire damage.
		public override IEnumerator Play()
		{
			List<DestroyCardAction> destroyed = new List<DestroyCardAction>();
			LinqCardCriteria linq = new LinqCardCriteria((Card c) => c.IsEnvironment && !c.IsTarget, "Non-target environment card");
			IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, linq, false, destroyed, base.CharacterCard, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if (!DidDestroyCard(destroyed))
            {
				coroutine = DealDamage(base.CharacterCard, (Card card) => !IsHero(card) || card == base.CharacterCard, 2, DamageType.Fire);
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
