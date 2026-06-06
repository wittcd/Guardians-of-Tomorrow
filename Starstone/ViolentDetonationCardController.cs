using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class ViolentDetonationCardController : CardController
	{
		public ViolentDetonationCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddTrigger((DestroyCardAction d) => GameController.DoesCardContainKeyword(d.CardToDestroy.Card, "stone"), DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
		}

		private IEnumerator DealDamageResponse(DestroyCardAction d)
		{
			IEnumerator coroutine = DealDamage(base.CharacterCard, (Card card) => !card.IsHero, 1, DamageType.Projectile);
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