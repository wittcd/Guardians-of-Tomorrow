using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Nyctophobia
{
    internal class StalkPreyCardController: CardController
    {
        public StalkPreyCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<SelectTargetDecision> selectedCards = new List<SelectTargetDecision>();
            IEnumerator coroutine = GameController.SelectTargetAndStoreResults(DecisionMaker, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsTarget), selectedCards, selectionType: SelectionType.IncreaseDamageTaken, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (selectedCards.FirstOrDefault() != null)
            {
                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
                idse.UntilStartOfNextTurn(TurnTaker);
                idse.SourceCriteria.IsSpecificCard = CharacterCard;
                idse.TargetCriteria.IsSpecificCard = selectedCards.FirstOrDefault().SelectedCard;
                coroutine = GameController.AddStatusEffectEx(idse, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
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
