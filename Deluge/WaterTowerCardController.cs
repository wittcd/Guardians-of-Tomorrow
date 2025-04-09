using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Deluge
{
    class WaterTowerCardController : CardController
    {
        public WaterTowerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddReduceDamageTrigger((Card c) => c.IsVillain && c != Card, 1);
            AddWhenDestroyedTrigger((DestroyCardAction dc) => PlayFloodWhenDestroyed(), TriggerType.PutIntoPlay);
        }

        private IEnumerator PlayFloodWhenDestroyed() 
        {
			bool tryPlaying = true;
			string message = base.Card.Title + " plays a Flood card from the Trash.";
			if (!base.TurnTaker.Trash.Cards.Any((Card c) => c.DoKeywordsContain("flood")))
			{
				tryPlaying = false;
				message = "There are no Flood cards in Deluge's trash for her to play.";
			}
			IEnumerator coroutine = base.GameController.SendMessageAction(message, Priority.Medium, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (tryPlaying)
			{
				IEnumerator coroutine2 = ReviveCardFromTrash(base.TurnTakerController, (Card c) => c.DoKeywordsContain("flood"));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
		}


    }
}
