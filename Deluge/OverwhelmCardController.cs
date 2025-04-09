using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Deluge
{
    class OverwhelmCardController : CardController
    {
        public OverwhelmCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHighestHP(1, () => 1, new LinqCardCriteria((Card c) => IsVillainTarget(c), "villain target"));
            SpecialStringMaker.ShowHeroWithMostCards(true);
        }

        public override IEnumerator Play()
        {
            List<Card> highestVillain = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetsWithHighestHitPoints(1, 1, (Card c) => IsVillainTarget(c), highestVillain, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<TurnTaker> mostHero = new List<TurnTaker>();
            coroutine = FindHeroWithMostCardsInHand(mostHero);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidFindCard(highestVillain) && mostHero.FirstOrDefault() != null)
            {
                coroutine = DealDamage(highestVillain.FirstOrDefault(), mostHero.FirstOrDefault().CharacterCard, H - 1, DamageType.Melee, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                CannotPlayCardsStatusEffect cpcse = new CannotPlayCardsStatusEffect();
                cpcse.TurnTakerCriteria.IsSpecificTurnTaker = mostHero.FirstOrDefault();
                cpcse.UntilStartOfNextTurn(TurnTaker);
                coroutine = AddStatusEffect(cpcse);
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
