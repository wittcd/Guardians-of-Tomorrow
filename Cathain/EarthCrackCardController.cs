using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace GuardiansOfTomorrow.Cathain
{
    public class EarthCrackCardController : CardController
    {
        public EarthCrackCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"{Cathain} deals 1 target 3 melee damage then deals that target 2 melee damage.
            List<SelectTargetDecision> selectedTargets = new List<SelectTargetDecision>();
            IEnumerator coroutine = GameController.SelectTargetAndStoreResults(DecisionMaker, FindCardsWhere((Card c) => c.IsTarget && c.IsInPlayAndHasGameText), selectedTargets, selectionType: SelectionType.DealDamage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (selectedTargets.Count > 0)
            {
                Card target = selectedTargets.FirstOrDefault().SelectedCard;
                coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c == target, 3, DamageType.Melee, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c == target, 2, DamageType.Melee, cardSource: GetCardSource());
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
