using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Kumiho
{
	class SoulEmbersCardController : CardController
	{
		public SoulEmbersCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			SpecialStringMaker.ShowHeroWithMostCards(false);
		}

        public override void AddTriggers()
        {
			AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageMaybeMoveCardResponse, TriggerType.DealDamage);
            base.AddTriggers();
        }

		private IEnumerator DealDamageMaybeMoveCardResponse(PhaseChangeAction pc)
        {
			List<TurnTaker> most = new List<TurnTaker>();
			IEnumerator coroutine = FindHeroWithMostCardsInPlay(most);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			coroutine = DealDamage(base.CharacterCard, most.FirstOrDefault().CharacterCard, 1, DamageType.Fire, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			List<Card> findSoul = FindCardsWhere((Card c) => c.Identifier == "StolenSoul" && c.IsInPlayAndHasGameText).ToList();
			if (findSoul.Count() > 0)
			{
				Card soul = findSoul.FirstOrDefault();
				coroutine = GameController.MoveCard(base.TurnTakerController, most.FirstOrDefault().Deck.TopCard, soul.UnderLocation, showMessage: true, cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			} 
			else
            {
				coroutine = DealDamage(base.CharacterCard, most.FirstOrDefault().CharacterCard, 2, DamageType.Fire, cardSource: GetCardSource());
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
