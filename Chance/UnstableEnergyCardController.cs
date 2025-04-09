using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class UnstableEnergyCardController : ChanceCardController
    {
        public UnstableEnergyCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
			AllowFastCoroutinesDuringPretend = false;
		}

        public override void AddTriggers()
        {
			AddTrigger((UsePowerAction p) => base.TurnTakerController != null && p.Power.TurnTakerController == base.TurnTakerController && p.Power.CardController != null && p.Power.Index >= 0 && p.Power.CardSource.CardController.CardWithoutReplacements.Location.HighestRecursiveLocation == base.TurnTaker.PlayArea && ((p.Power.CopiedFromCardController != null && p.Power.CopiedFromCardController.HasPowerNumerals()) || p.Power.CardController.HasPowerNumerals()), ModifyNumeralResponse, TriggerType.ModifyNumeral, TriggerTiming.Before);
		}

		private IEnumerator ModifyNumeralResponse(UsePowerAction p)
		{
			CardController cc = ((p.Power.CopiedFromCardController != null) ? p.Power.CopiedFromCardController : ((p.Power.CardSource.CardController == null || p.Power.CardSource.CardController == p.Power.CardController) ? p.Power.CardController : p.Power.CardSource.CardController));
			IEnumerable<string> powerNumeralStrings = cc.GetPowerNumeralStrings(p.Power, p.Power.Index);
			List<SelectWordDecision> storedResults = new List<SelectWordDecision>();
			Card[] associatedCards = new Card[1] { cc.Card };
			IEnumerator coroutine = base.GameController.SelectWord(DecisionMaker, powerNumeralStrings, SelectionType.SelectNumeral, storedResults, optional: true, associatedCards, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			SelectWordDecision selectWordDecision = storedResults.FirstOrDefault();
			if (selectWordDecision != null && selectWordDecision.Index.HasValue)
			{
				int index = selectWordDecision.Index.Value;
				int num = cc.GetPowerNumerals(p.Power, p.Power.Index).ElementAt(index);
				string displayText = $"Increase by 2: {selectWordDecision.SelectedWord.Replace(num.ToString(), (num + 2).ToString())}";
				string displayText2 = $"Reduce by 2: {selectWordDecision.SelectedWord.Replace(num.ToString(), (num - 2).ToString())}";
				IEnumerable<Function> functionChoices = new Function[2]
				{
					new Function(base.HeroTurnTakerController, displayText, SelectionType.ModifyNumeral, () => ModifyFunction(p.Power, index, 2)),
					new Function(base.HeroTurnTakerController, displayText2, SelectionType.ModifyNumeral, () => ModifyFunction(p.Power, index, -2))
				};
				SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, base.HeroTurnTakerController, functionChoices, optional: true, null, null, associatedCards, GetCardSource());
				IEnumerator coroutine2 = base.GameController.SelectAndPerformFunction(selectFunction, null, associatedCards);
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

		private IEnumerator ModifyFunction(Power p, int numeralIndex, int amount)
		{
			CardController cardController = p.CardController;
			string identifier = p.CardController.Card.Identifier;
			if (p.CopiedFromCardController != null)
			{
				cardController = p.CopiedFromCardController;
			}
			else if (p.IsContributionFromCardSource)
			{
				cardController = p.CardSource.CardController;
				identifier = cardController.Card.Identifier;
			}
			cardController.AddPowerNumeralModification(identifier, numeralIndex, amount);
			//yield return null;

			List<int> storedResultsCoin = new List<int>();
			IEnumerator coroutine = FlipCoin(storedResultsCoin, true, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if (storedResultsCoin.Count() == 1 && storedResultsCoin.FirstOrDefault() == 0)
            {
				coroutine = DestroyThisCardResponse(null);
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
