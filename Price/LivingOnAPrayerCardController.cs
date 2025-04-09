using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Price
{
    class LivingOnAPrayerCardController : CardController
    {
        public LivingOnAPrayerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DealDamageAction> storedresults = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Radiant, 3, false, 0, storedResultsDamage: storedresults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            int totalTargets = 0;

            List<Card> targets = new List<Card>();

            foreach (DealDamageAction dd in storedresults)
            {
                if (!targets.Contains(dd.Target) && dd.DidDealDamage)
                {
                    totalTargets++;
                    targets.Add(dd.Target);
                }
            }

            int healamount = 4 - totalTargets;
            coroutine = GameController.GainHP(CharacterCard, healamount, cardSource: GetCardSource());
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
