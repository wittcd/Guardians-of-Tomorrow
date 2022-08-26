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

            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, ds, 1, DamageType.Fire, 1, false, 1, storedResultsDamage: srd1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //srdFull.Add(srd3.FirstOrDefault());

            /*Card c1 = null;
            Card c2 = null;
            Card c3 = null;


            if (DidDealDamage(srd1))
            {
                c1 = srd1.FirstOrDefault().Target;
            }
            if (DidDealDamage(srd2))
            {
                c2 = srd2.FirstOrDefault().Target;
            }
            if (DidDealDamage(srd3))
            {
                c3 = srd3.FirstOrDefault().Target;
            }*/

            

            //int damageToDeal = 4 - srdFull.Where((DealDamageAction dd) => dd.DidDealDamage).Select((DealDamageAction dd) => dd.Target).Distinct().Count();

            /*if (c1 != null && c2 != null && c1 != c2)
            {
                damageToDeal--;
            }
            if (c2 != null && c3 != null && c2 != c3)
            {
                damageToDeal--;
            }
            if (c3 != null && c1 != null && c3 != c1)
            {
                damageToDeal--;
            }*/

            coroutine = DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Fire, cardSource: GetCardSource());
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
