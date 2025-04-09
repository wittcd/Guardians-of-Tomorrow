using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.Ludramor
{
    class SurveyDepartmentCardController : CardController
    {
        public SurveyDepartmentCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => SelectPlayerToDiscard(), TriggerType.DiscardCard);
        }

        private IEnumerator SelectPlayerToDiscard()
        {
            IEnumerator coroutine = GameController.SelectTurnTakerAndDoAction(new SelectTurnTakerDecision(GameController, DecisionMaker, FindTurnTakersWhere((TurnTaker tt) => tt.IsHero), SelectionType.DiscardCard, cardSource: GetCardSource()), (TurnTaker tt) => GameController.SelectAndDiscardCard(FindTurnTakerController(tt).ToHero(), cardSource: GetCardSource()));
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
