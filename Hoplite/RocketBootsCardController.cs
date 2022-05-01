using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Hoplite
{

	public class RocketBootsCardController : CardController
	{

		public RocketBootsCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard, (DealDamageAction dd) => ReduceOrDrawResponse(dd), TriggerType.Other, TriggerTiming.Before);
		}

		public IEnumerator ReduceOrDrawResponse(DealDamageAction dd)
		{
			List<Function> options = new List<Function>();
			Function reduce = new Function(DecisionMaker, "Reduce Damage", SelectionType.ReduceDamageTaken, () => GameController.ReduceDamage(dd, 1, null));
			options.Add(reduce);
			Function draw = new Function(DecisionMaker, "Draw a card", SelectionType.DrawCard, () => GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource()));
			options.Add(draw);
			SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, DecisionMaker, options, optional: false, null, null, null, GetCardSource());
			IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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