using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Kumiho
{
	class DarkDomainCardController : CardController
	{
		public DarkDomainCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

        public override void AddTriggers()
        {
			AddReduceDamageTrigger((Card c) => c.IsVillain, 1);
			AddTrigger((DestroyCardAction d) => d.WasCardDestroyed && d.PostDestroyDestinationCanBeChanged && d.CardToDestroy.Card.DoKeywordsContain("ghost"), GainHPFromDeathResponse, TriggerType.MoveCard, TriggerTiming.After);
			base.AddTriggers();
        }

		private IEnumerator GainHPFromDeathResponse(DestroyCardAction dc)
        {
			Card card = dc.CardToDestroy.Card;
			IEnumerator coroutine = GameController.GainHP(base.CharacterCard, 3, null, null, GetCardSource());
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