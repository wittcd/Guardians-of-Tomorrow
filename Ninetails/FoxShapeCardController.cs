using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class FoxShapeCardController : CardController
	{

		private ITrigger _modifyDamageAmount;

		private DealDamageAction DealDamageAction { get; set; }

		private SelectFunctionDecision SelectFunctionDecision { get; set; }

		private Card Target { get; set; }

		public FoxShapeCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers() 
		{
			AddTrigger((RedirectDamageAction rd) => rd.OldTarget == base.CharacterCard, (RedirectDamageAction rd) => ModifyDamageAmountResponse(rd), TriggerType.ModifyDamageAmount, TriggerTiming.After);
		}

		private IEnumerator IncreaseFunction()
		{
			return base.GameController.IncreaseDamage(DealDamageAction, 1, isNemesisEffect: false, GetCardSource());
		}

		private IEnumerator ReduceFunction()
		{
			return base.GameController.ReduceDamage(DealDamageAction, 1, _modifyDamageAmount, GetCardSource());
		}

		private IEnumerator ModifyDamageAmountResponse(RedirectDamageAction rd)
		{
			DealDamageAction = rd.DealDamageAction;
			if (base.GameController.PretendMode || DealDamageAction.Target != Target)
			{
				IEnumerable<Function> functionChoices = new Function[2]
				{
					new Function(base.HeroTurnTakerController, "Increase by 1", SelectionType.IncreaseDamage, IncreaseFunction),
					new Function(base.HeroTurnTakerController, "Reduce by 1", SelectionType.ReduceDamageTaken, ReduceFunction)
				};
				List<SelectFunctionDecision> selectFunction = new List<SelectFunctionDecision>();
				SelectFunctionDecision selectFunction2 = new SelectFunctionDecision(base.GameController, base.HeroTurnTakerController, functionChoices, optional: false, rd.DealDamageAction, null, null, GetCardSource());
				IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction2, selectFunction);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				if (selectFunction.Count() > 0)
				{
					SelectFunctionDecision = selectFunction.FirstOrDefault();
				}
				Target = rd.DealDamageAction.Target;
			}
			else if (SelectFunctionDecision.SelectedFunction != null)
			{
				IEnumerator coroutine2 = SelectFunctionDecision.SelectedFunction.FunctionToExecute();
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			if (!base.GameController.PretendMode)
			{
				SelectFunctionDecision = null;
				DealDamageAction = null;
			}
		}
	}
}