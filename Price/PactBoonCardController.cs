using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Price
{
    class PactBoonCardController : CardController
    {
        public PactBoonCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.DamageType == DamageType.Infernal, 1);
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.DamageType == DamageType.Radiant, (DealDamageAction dd) => 1);
            AddReduceDamageTrigger((DealDamageAction dd) => dd.Target == CharacterCard && dd.DamageType == DamageType.Radiant, (DealDamageAction dd) => 1);

        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeralTargets = GetPowerNumeral(0, 3);
            int powerNumeralDamage = GetPowerNumeral(1, 2);
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), powerNumeralDamage, DamageType.Infernal, powerNumeralTargets, false, 0, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(DecisionMaker, TurnTaker.Deck, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("pact"), "pact"));
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
