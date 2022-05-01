using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class BlackHoleCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public BlackHoleCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.Target.IsCharacter, (DealDamageAction dd) => DiscardTopCardResponse(dd), TriggerType.DiscardCard, TriggerTiming.After);
		}

		public IEnumerator DiscardTopCardResponse(DealDamageAction dd)
		{
			IEnumerator coroutine = DiscardCardsFromTopOfDeck(GameController.FindTurnTakerController(dd.Target.Owner), 1);
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