using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class WeighOptionsCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public WeighOptionsCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			
			for (int i = 0; i < 2; i++)
			{
				List<Function> list = new List<Function>();
				Function damage = new Function(DecisionMaker, "{Scale} deals 1 target 2 energy damage", SelectionType.DealDamage, () => GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), 2, DamageType.Energy, 1, false, 1, cardSource: GetCardSource()));
				Function draw = new Function(DecisionMaker, "Draw a card", SelectionType.DrawCard, () => GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource()));
				Function destroy = new Function(DecisionMaker, "Destroy an ongoing or environment card", SelectionType.DestroyCard, () => GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsOngoing || c.IsEnvironment, "ongoing or environment"), optional: false, null, null, GetCardSource()));
				Function heal = new Function(DecisionMaker, "{Scale} regains 2 HP", SelectionType.GainHP, () => GameController.GainHP(base.CharacterCard, 2, null, null, GetCardSource()));
				list.Add(draw);
				list.Add(damage);
				list.Add(destroy);
				list.Add(heal);
				SelectFunctionDecision sf = new SelectFunctionDecision(GameController, DecisionMaker, list, false, cardSource: GetCardSource());
				IEnumerator coroutine = base.GameController.SelectAndPerformFunction(sf);
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