using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class MetamorphicArmorCardController : CardController
	{
		public MetamorphicArmorCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("stone"), "stone"));
		}

		public override void AddTriggers()
		{
			AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
			AddReduceDamageTrigger((Card c) => c == base.CharacterCard, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("stone", false, false)).Count());
		}

		public override IEnumerator Play()
		{
			MoveCardDestination obj = new MoveCardDestination(base.HeroTurnTakerController.HeroTurnTaker.Hand);
			IEnumerator coroutine = SearchForCards(DecisionMaker, true, false, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("stone")), true, false, false, false, null, false, true, null);
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