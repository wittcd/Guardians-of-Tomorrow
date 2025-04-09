using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Nyctophobia
{
    internal class QuickStabCardController : CardController
    {
        public QuickStabCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            
        }

        public override IEnumerator Play()
        {
            List<DealDamageAction> storedDamage = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Melee, 1, false, 1, storedResultsDamage: storedDamage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedDamage.FirstOrDefault().DidDestroyTarget)
            {
                coroutine = GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.SelectAndPlayCardFromHand(TurnTakerController.ToHero(), true, cardSource: GetCardSource());
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
