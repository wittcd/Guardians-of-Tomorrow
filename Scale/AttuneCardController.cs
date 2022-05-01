using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class AttuneCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public AttuneCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//move any number of balance from trash to hand
			LinqCardCriteria isBalance = new LinqCardCriteria((Card c) => c.DoKeywordsContain("balance"), "balance cards");
			MoveCardDestination obj = new MoveCardDestination(base.HeroTurnTakerController.HeroTurnTaker.Hand);
			IEnumerator coroutine = GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, base.HeroTurnTakerController.TurnTaker.Trash, 0, 6, isBalance, obj.ToEnumerable(), cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//deal 1 target 3 energy damage
			coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), 3, DamageType.Energy, 1, false, 0, cardSource: GetCardSource());
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