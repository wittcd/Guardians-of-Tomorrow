using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Kumiho
{
    class GhoulishHuntersCardController : CardController
    {
        public GhoulishHuntersCardController(Card card, TurnTakerController turnTakerController)
               : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageAndDoExtraOnKillResponse, TriggerType.DealDamage);
            base.AddTriggers();
        }

        private IEnumerator DealDamageAndDoExtraOnKillResponse(PhaseChangeAction pc)
        {
            List<DealDamageAction> storedResultsA = new List<DealDamageAction>();
            List<DealDamageAction> storedResultsB = new List<DealDamageAction>();
            List<DealDamageAction> damages = new List<DealDamageAction>();
            damages.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, base.Card), null, 1, DamageType.Melee, false, null, storedResultsA, null, false));
            damages.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, base.Card), null, 1, DamageType.Cold, false, null, storedResultsB, null, false));

            IEnumerator coroutine = DealMultipleInstancesOfDamageToHighestLowestHP(damages, (Card c) => c.IsHero, HighestLowestHP.LowestHP, 1, 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if ((DidDealDamage(storedResultsB) && storedResultsB.FirstOrDefault().DidDestroyTarget) || (DidDealDamage(storedResultsA) && storedResultsA.FirstOrDefault().DidDestroyTarget))
            {
                coroutine = GameController.SetHP(base.Card, 5, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = PlayTheTopCardOfTheVillainDeckResponse(null);
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
                coroutine = DoNothing();
            }
        }
    }
}
