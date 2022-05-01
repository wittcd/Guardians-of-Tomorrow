using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class ScientificApproachToMagicCardController : CardController
	{
		public ScientificApproachToMagicCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers() 
		{
			AddTrigger((DiscardCardAction dc) => dc.ResponsibleTurnTaker == base.TurnTaker, (DiscardCardAction dc) => GainHPResponse(dc), TriggerType.GainHP, TriggerTiming.After);
		}

		private IEnumerator GainHPResponse(DiscardCardAction dc)
		{
			IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, 1, null, null, GetCardSource());
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