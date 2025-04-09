using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Ludramor
{
    class QuashResistanceCardController : CardController
    {
        public QuashResistanceCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("soldier"), "soldier"));
            SpecialStringMaker.ShowIfSpecificCardIsInPlay("ColonelPrenakka");
            SpecialStringMaker.ShowHighestHP(1, () => numSoldier(), new LinqCardCriteria((Card c) => IsHero(c)));
        }

        private int numSoldier()
        {
            return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("soldier")).Count();
        }

        public override IEnumerator Play()
        {
            //check if Prenakka in play
            if (FindCardsWhere((Card c) => c.Identifier == "ColonelPrenakka" && c.IsInPlay).Count() > 0)
            {
                Card p = FindCardsWhere((Card c) => c.Identifier == "ColonelPrenakka" && c.IsInPlay).FirstOrDefault();
                List<Card> highest = new List<Card>();
                IEnumerator coroutine = GameController.FindTargetsWithHighestHitPoints(1, numSoldier(), (Card c) => IsHero(c), highest, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.DealDamage(DecisionMaker, p, (Card c) => highest.Contains(c), 2, DamageType.Energy, cardSource: GetCardSource());
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
                IEnumerator coroutine;
                List<Card> prenakkaLocation = (List<Card>)FindCardsWhere((Card c) => c.Identifier == "ColonelPrenakka" && (c.Location == TurnTaker.Trash || c.Location == TurnTaker.Deck));
                if (prenakkaLocation.Count() > 0)
                {
                    coroutine = GameController.PlayCard(TurnTakerController, prenakkaLocation.FirstOrDefault(), true, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                coroutine = GameController.ShuffleLocation(TurnTaker.Deck, cardSource: GetCardSource());
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
