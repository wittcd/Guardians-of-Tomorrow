using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class LuckyFindCardController : ChanceCardController
    {
        public LuckyFindCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<int> coin = new List<int>();
            IEnumerator coroutine = FlipCoin(coin, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (coin.Count() > 0)
            {
                LinqCardCriteria cardCriteria = new LinqCardCriteria((Card c) => IsOngoing(c) || IsEquipment(c));
                if (coin.FirstOrDefault() == 1)
                {
                    List<MoveCardDestination> dest = new List<MoveCardDestination>();
                    dest.Add(new MoveCardDestination(TurnTaker.ToHero().Hand));
                    coroutine = GameController.SelectCardFromLocationAndMoveIt(DecisionMaker, TurnTaker.Deck, cardCriteria, dest, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else
                {
                    coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(DecisionMaker, TurnTaker.Deck, false, true, false, cardCriteria, 1);
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
