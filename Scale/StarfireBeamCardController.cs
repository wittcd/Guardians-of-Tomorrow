using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class StarfireBeamCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public StarfireBeamCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator UsePower(int index = 0)
		{
			IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(base.TurnTakerController.ToHero(), new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Fire, 1, false, 1, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			LinqCardCriteria isBalanceCard = new LinqCardCriteria((Card c) => c.DoKeywordsContain("balance", false, false), "balance");
			List<Function> list = new List<Function>();
			Function playBalance = new Function(DecisionMaker, "Play a balance card", SelectionType.PlayCard, () => SelectAndPlayCardFromHand(DecisionMaker, false, null, isBalanceCard, false, false, true, null));
			list.Add(playBalance);
			Function destroyBalance = new Function(DecisionMaker, "Destroy a balance card", SelectionType.DestroyCard, () => GameController.SelectAndDestroyCard(DecisionMaker, isBalanceCard, false, null, null, GetCardSource()), null, null, null);
			list.Add(destroyBalance);
			SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, DecisionMaker, list, optional: true, null, null, null, GetCardSource());
			coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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