using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Deluge
{
    class BurstDamCardController : CardController
    {
        public BurstDamCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddImmuneToDamageTrigger((DealDamageAction dd) => dd.Target.DoKeywordsContain("flood"));
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DiscardAndDrawToDestroy(pc), TriggerType.DiscardCard);
        }

        private IEnumerator DiscardAndDrawToDestroy(PhaseChangeAction pc)
        {
            List<DiscardCardAction> discards = new List<DiscardCardAction>();
            List<SelectTurnTakerDecision> discarder = new List<SelectTurnTakerDecision>();
            IEnumerator coroutine = GameController.SelectHeroToDiscardTheirHand(DecisionMaker, true, false, storedResultsTurnTaker: discarder, storedResultsDiscard: discards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDiscardCards(discards)) 
            {
                coroutine = DrawCards(FindTurnTakerController(discarder.FirstOrDefault().SelectedTurnTaker).ToHero(), 2);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidDiscardCards(discards, 3, true)) 
                {
                    coroutine = DestroyThisCardResponse(pc);
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
}
