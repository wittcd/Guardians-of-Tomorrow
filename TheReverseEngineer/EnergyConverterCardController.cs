using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using GuardiansOfTomorrow.TheReverseEngineer;

namespace GuardiansOfTomorrow.TheReverseEngineer
{
    class EnergyConverterCardController : TheReverseEngineerCardController
    {
        public EnergyConverterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator FromTrashAction()
        {
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c), "ongoing"), true, null, CharacterCard, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            AddTrigger((DealDamageAction dd) => dd.DidDealDamage && dd.Target == CharacterCard && dd.Amount >= 3, RevealMaybeDiscardAndThenDealDamage, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator RevealMaybeDiscardAndThenDealDamage(DealDamageAction dd)
        {
            IEnumerator coroutine = RevealTopCard_PutItBackOrDiscardIt(DecisionMaker, TurnTakerController, TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DealDamage(CharacterCard, (Card c) => IsVillain(c), 1, DamageType.Energy);
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
