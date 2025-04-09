using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Angler
{
    public class SetLinesCardController : CardController
    {

        public SetLinesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = GameController.SelectCardFromLocationAndMoveIt(DecisionMaker, TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("bait"), "bait"), new MoveCardDestination[] { new MoveCardDestination(TurnTaker.ToHero().Hand) }, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectAndPlayCardsFromHand(FindHeroTurnTakerController(TurnTaker.ToHero()), FindCardsWhere((Card c) => c.Location == TurnTaker.ToHero().Hand && c.DoKeywordsContain("bait")).Count(), false, 0, new LinqCardCriteria((Card c) => c.DoKeywordsContain("bait"), "bait"), cardSource: GetCardSource(), cancelDecisionsIfTrue: () => FindCardsWhere((Card c) => c.Location == TurnTaker.ToHero().Hand && c.DoKeywordsContain("bait")).Count() == 0);
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
