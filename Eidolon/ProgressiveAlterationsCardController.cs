using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class ProgressiveAlterationsCardController : CardController
    {
        public ProgressiveAlterationsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("alteration"), "alteration"));
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = GameController.ShuffleCardsIntoLocation(DecisionMaker, FindCardsWhere((Card c) => c.DoKeywordsContain("alteration") && c.Location == base.TurnTaker.Trash), base.TurnTaker.Deck, false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = PlayTheTopCardOfTheVillainDeckResponse(null);
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
