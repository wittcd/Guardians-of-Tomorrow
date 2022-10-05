using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class AlterationCardController : CardController
    {
        public AlterationCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        public IEnumerator AlterationPlayedResponse(DamageType DamageTypeToDeal, IEnumerator IfDamagedResponse)
        {
            List<DealDamageAction> StoredDamage = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageTypeToDeal, false, false, false, null, StoredDamage, null, false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(StoredDamage) && StoredDamage.FirstOrDefault().Target == base.CharacterCard)
            {
                coroutine = IfDamagedResponse;
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
                coroutine = DealDamage((Card c) => c == base.CharacterCard, (Card c) => !c.IsVillain, (Card c) => 2, DamageTypeToDeal);
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
