using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Nyctophobia
{
    internal class SwordOfEhudCardController : CardController
    {
        public SwordOfEhudCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.DamageType == DamageType.Melee, 1);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeralTargets = GetPowerNumeral(0, 1);
            int powerNumeralDamage = GetPowerNumeral(1, 2);
            List<DealDamageAction> storedDamage = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), powerNumeralDamage, DamageType.Melee, powerNumeralTargets, false, powerNumeralTargets, storedResultsDamage: storedDamage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            bool didDestroy = false;
            foreach (DealDamageAction dd in storedDamage)
            {
                if (!didDestroy && dd.DidDestroyTarget)
                {
                    didDestroy = true;
                    List<Function> list = new List<Function>();
                    Function item = new Function(DecisionMaker, "Draw a card", SelectionType.DrawCard, () => DrawCard(TurnTaker.ToHero()), null, null, "Draw a card");
                    list.Add(item);
                    Function item2 = new Function(DecisionMaker, "Play a card", SelectionType.PlayCard, () => SelectAndPlayCardsFromHand(DecisionMaker, 1, optional: false, cardCriteria: new LinqCardCriteria((Card c) => true, "card", false), requiredDecisions: 0), base.HeroTurnTaker.Hand.Cards.Count() > 0, null, "Play a card");
                    list.Add(item2);
                    SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, DecisionMaker, list, optional: false, null, null, null, GetCardSource());
                    coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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

}
