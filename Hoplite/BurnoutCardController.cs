using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Hoplite
{
    class BurnoutCardController : CardController
    {
        public BurnoutCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            //{Hoplite} deals 1 target 3 fire damage, then 1 target 2 fire damage,
            //then 1 target 1 fire damage. Then {Hoplite} deals himself fire damage
            //equal to 4 minus the number of targets dealt damage this way.

            DamageSource ds = new DamageSource(GameController, base.CharacterCard);
            List<DealDamageAction> srd1 = new List<DealDamageAction>();
            List<DealDamageAction> srd2 = new List<DealDamageAction>();
            List<DealDamageAction> srd3 = new List<DealDamageAction>();

            //List<DealDamageAction> srdFull = new List<DealDamageAction>();

            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, ds, 3, DamageType.Fire, 1, false, 1, storedResultsDamage: srd1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //srdFull.Add(srd1.FirstOrDefault());

            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, ds, 2, DamageType.Fire, 1, false, 1, storedResultsDamage: srd2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //srdFull.Add(srd2.FirstOrDefault());

            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, ds, 1, DamageType.Fire, 1, false, 1, storedResultsDamage: srd3, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<DealDamageAction> finalStoredResults = new List<DealDamageAction>();
            finalStoredResults.AddRange(srd1);
            finalStoredResults.AddRange(srd2);
            finalStoredResults.AddRange(srd3);

            /*coroutine = GameController.SendMessageAction("Successfully concatenated", Priority.Critical, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }*/

            //int totalTargets = GetNumberOfTargetsDealtDamage(finalStoredResults);

            int totalTargets = 0;

            List<Card> targets = new List<Card>();

            foreach (DealDamageAction dd in finalStoredResults)
            {
                if (!targets.Contains(dd.Target) && dd.DidDealDamage)
                {
                    totalTargets++;
                    targets.Add(dd.Target);
                }
            }

            /*coroutine = GameController.SendMessageAction("Successfully Calculated target count", Priority.Critical, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }*/

            coroutine = DealDamage(base.CharacterCard, base.CharacterCard, 4 - totalTargets, DamageType.Fire, cardSource: GetCardSource());
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
