using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheHatter
{
    class WhiteRabbitCardController : CardController
    {
        public WhiteRabbitCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroWithMostCards(true);
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, HeroWithMostMustDiscard, TriggerType.DiscardCard);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pc) => DealDamage((Card c) => c == base.Card, (Card c) => c.DoKeywordsContain("thrall") && c != base.Card, (Card c) => 1, DamageType.Sonic), TriggerType.DealDamage);
            base.AddTriggers();
        }

        private IEnumerator HeroWithMostMustDiscard(PhaseChangeAction pc)
        {
            List<TurnTaker> mostCards = new List<TurnTaker>();
            IEnumerator coroutine = FindHeroWithMostCardsInHand(mostCards);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (mostCards.FirstOrDefault() != null)
            {
                TurnTakerController controller = FindTurnTakerController(mostCards.FirstOrDefault());
                coroutine = GameController.SelectAndDiscardCards(controller.ToHero(), 2, false, 2, cardSource: GetCardSource());
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
