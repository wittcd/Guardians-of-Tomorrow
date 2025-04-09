using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class SpiritOfDeceptionCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public SpiritOfDeceptionCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//end of turn destroy ongoing and if so, destroy this
			AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, EndOfTurnResponse, TriggerType.DestroyCard);
			//deal 2 infernal to all non-heroes
			AddWhenDestroyedTrigger(OnDestroyResponse, TriggerType.DealDamage);
		}

		private IEnumerator OnDestroyResponse(DestroyCardAction dc)
		{
			IEnumerator coroutine = DealDamage(base.CharacterCard, (Card card) => !IsHero(card), 1, DamageType.Psychic, true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public IEnumerator EndOfTurnResponse(PhaseChangeAction p)
		{
			List<DestroyCardAction> storedDestroy = new List<DestroyCardAction>();
			IEnumerator coroutine = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c), "ongoing"), true, storedDestroy, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (DidDestroyCard(storedDestroy))
			{
				coroutine = base.GameController.DestroyCard(DecisionMaker, base.Card, false, null, null, null, null, null, null, null, null, GetCardSource());
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