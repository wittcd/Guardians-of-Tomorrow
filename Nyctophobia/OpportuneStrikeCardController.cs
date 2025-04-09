using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Nyctophobia
{
    internal class OpportuneStrikeCardController : CardController
    {
        public OpportuneStrikeCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //{Nyctophobia} deals 1 target 2 melee damage. Draw X cards where X is half the amount of damage dealt this way, rounded up.
            List<DealDamageAction> storedDamage = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Melee, 1, false, 1, storedResultsDamage: storedDamage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(storedDamage))
            {
                int cardsToDraw = storedDamage.FirstOrDefault().Amount / 2 + storedDamage.FirstOrDefault().Amount % 2;
                coroutine = GameController.DrawCards(TurnTakerController.ToHero(), cardsToDraw, cardSource: GetCardSource());
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
