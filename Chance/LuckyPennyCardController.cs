using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class LuckyPennyCardController : ChanceCardController
    {
        public LuckyPennyCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<Function> functions = new List<Function>();
            functions.Add(new Function(DecisionMaker, "Draw a card", SelectionType.DrawCard, () => DrawCard(base.TurnTaker.ToHero())));
            functions.Add(new Function(DecisionMaker, "Use a power", SelectionType.UsePower, () => SelectAndUsePower(FindCardController(base.CharacterCard))));
            SelectFunctionDecision drawOrPower = new SelectFunctionDecision(GameController, DecisionMaker, functions, true, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(drawOrPower);
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
