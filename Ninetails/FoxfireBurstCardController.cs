using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class FoxfireBurstCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public FoxfireBurstCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}



		public override IEnumerator Play()
		{
			//deal each non-hero target 2 fire damage
			IEnumerator coroutine = DealDamage(base.CharacterCard, (Card card) => !card.IsHero, 2, DamageType.Fire);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//discard. If so, play.
			List<DiscardCardAction> srd = new List<DiscardCardAction>();
			coroutine = SelectAndDiscardCards(DecisionMaker, 1, optional: true, null, srd);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if (DidDiscardCards(srd))
			{
				coroutine = SelectAndPlayCardFromHand(DecisionMaker);
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