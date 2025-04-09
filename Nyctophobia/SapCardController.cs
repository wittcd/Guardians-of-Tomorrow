using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Nyctophobia
{
    internal class SapCardController : CardController
    {
        public SapCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsInPlay(new LinqCardCriteria((Card c) => !c.IsCharacter && c.IsTarget && c.HitPoints <= 3, "targets with less than 3 HP", false));
        }

        public override IEnumerator Play()
        {
            SelectCardDecision scd = new SelectCardDecision(GameController, DecisionMaker, SelectionType.MoveCard, FindCardsWhere((Card c) => c.IsInPlayAndNotUnderCard && !c.IsCharacter && c.IsTarget && c.HitPoints <= 3), cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectCardAndDoAction(scd, MoveCardToItsDeckAndShuffle);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<Function> list = new List<Function>();
            Function item = new Function(DecisionMaker, "Draw a card", SelectionType.DrawCard, () => DrawCard(TurnTaker.ToHero()), null, null, "Draw a card");
            list.Add(item);
            Function item2 = new Function(DecisionMaker, "Play a card", SelectionType.PlayCard, () => SelectAndPlayCardsFromHand(DecisionMaker, 1, optional: false, cardCriteria: new LinqCardCriteria((Card c) => true, "card", false), requiredDecisions: 0), base.HeroTurnTaker.Hand.Cards.Count() > 0, null, "Play a card");
            list.Add(item2);
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, DecisionMaker, list, optional: false, null, null, null, GetCardSource());
            coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator MoveCardToItsDeckAndShuffle(SelectCardDecision s)
        {
            Card c = s.SelectedCard;
            Location d = c.Owner.Deck;

            IEnumerator coroutine = GameController.MoveCard(TurnTakerController, c, d, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.ShuffleLocation(d, cardSource: GetCardSource());
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
