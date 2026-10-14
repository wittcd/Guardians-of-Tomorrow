using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Cathain
{
    public class WindGraspCardController : CardController
    {
        public WindGraspCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<SelectTargetDecision> storedResults = new List<SelectTargetDecision>();
            IEnumerator coroutine = GameController.SelectTargetAndStoreResults(DecisionMaker, FindCardsWhere((Card c) => c.IsTarget && c.IsInPlay), storedResults, selectionType: SelectionType.ReduceDamageDealt ,cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults.Count > 0 && storedResults.FirstOrDefault().SelectedCard != null)
            {
                Card selected = storedResults.FirstOrDefault().SelectedCard;
                ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                rdse.UntilStartOfNextTurn(TurnTaker);
                rdse.SourceCriteria.IsSpecificCard = selected;
                coroutine = GameController.AddStatusEffectEx(rdse, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            coroutine = GameController.DrawCardsEx(new LinqTurnTakerCriteria((TurnTaker tt) => tt == TurnTaker), 1, true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectAndPlayCardFromHand(TurnTakerController.ToHero(), true, cardSource: GetCardSource());
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
