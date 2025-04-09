using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Legion
{
    class HackedVaultDoorCardController : CardController
    {
        public HackedVaultDoorCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddRedirectDamageTrigger((DealDamageAction dd) => IsVillain(dd.Target) && dd.Target != base.Card, () => base.Card);
            AddReduceDamageToSetAmountTrigger((DealDamageAction dd) => dd.Target == base.Card, 1);
            //AddTrigger((DealDamageAction dd) => dd.Target == base.Card && dd.DidDealDamage, (DealDamageAction dd) => OneHeroDiscardsToDestroyThisCardResponse(), new TriggerType[] { TriggerType.DiscardCard, TriggerType.DestroyCard }, TriggerTiming.After);
            AddEndOfTurnTrigger((TurnTaker tt) => IsHero(tt), (PhaseChangeAction pc) => AllHeroesDiscardCardsToDestroyThisCardResponse(), TriggerType.DiscardCard);
        }

        private IEnumerator AllHeroesDiscardCardsToDestroyThisCardResponse()
        {
            List<DiscardCardAction> stored = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.EachPlayerDiscardsCards(0, null, stored, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidDiscardCards(stored))
            {
                coroutine = DealDamage(Card, Card, stored.Count(), DamageType.Energy, true, cardSource: GetCardSource());
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
