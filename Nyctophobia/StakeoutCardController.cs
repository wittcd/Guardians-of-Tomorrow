using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Nyctophobia
{
    internal class StakeoutCardController: CardController
    {
        public StakeoutCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger((CardEntersPlayAction cep) => !IsHero(cep.CardEnteringPlay) && cep.CardEnteringPlay.IsTarget, increaseDamageToTargetUntilEndOfTurnThenDestroyThisCard, TriggerType.IncreaseDamage, TriggerTiming.After);
        }

        private IEnumerator increaseDamageToTargetUntilEndOfTurnThenDestroyThisCard(CardEntersPlayAction cep)
        {
            List<Card> assocCards = new List<Card>();
            assocCards.Add(cep.CardEnteringPlay);
            List<YesNoCardDecision> ynd = new List<YesNoCardDecision>();
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.Custom, Card, null, ynd, assocCards, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidPlayerAnswerYes(ynd)) {
                coroutine = GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(2);
                idse.UntilEndOfNextTurn(TurnTaker);
                idse.TargetCriteria.IsSpecificCard = cep.CardEnteringPlay;
                idse.SourceCriteria.IsSpecificCard = CharacterCard;

                coroutine = GameController.AddStatusEffectEx(idse, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            Card SelectedCard = decision.AssociatedCards.FirstOrDefault();
            return new CustomDecisionText(
                $"Increase damage dealt to {SelectedCard.Title} by Nyctophobia by 2, then draw a card and destroy this card?",
                $"The other heroes are deciding whether to increase damage dealt to {SelectedCard.Title} by Nyctophobia by 2, then draw a card and destroy this card",
                $"Vote for whether to increase damage dealt to {SelectedCard.Title} by Nyctophobia by 2, then draw a card and destroy this card",
                $"whether to increase damage dealt to {SelectedCard.Title} by Nyctophobia by 2, then draw a card and destroy this card"
            );
        }
    }
}
