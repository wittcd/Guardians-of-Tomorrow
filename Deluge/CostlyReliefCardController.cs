using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Deluge
{
    class CostlyReliefCardController : CardController
    {

        public CostlyReliefCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger((DestroyCardAction dc) => dc.CardToDestroy.Card.DoKeywordsContain("flood") && DidDestroyCard(dc), (DestroyCardAction dc) => EachHeroDealsSelfDamage(), TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator EachHeroDealsSelfDamage()
        {
            IEnumerator coroutine = GameController.DealDamageToSelf(DecisionMaker, (Card c) => IsHeroCharacterCard(c), (Card c) => 2, DamageType.Psychic, GetCardSource());
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
