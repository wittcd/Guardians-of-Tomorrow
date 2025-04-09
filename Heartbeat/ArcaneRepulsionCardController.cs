using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Heartbeat
{
    class ArcaneRepulsionCardController : CardController
    {

        public ArcaneRepulsionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.Identifier == "ArcaneCharge", "copies of Arcane Charge", false, false, "Copy of Arcane Charge", "Copies of Arcane Charge", false));
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int PowerNumeralReduce = GetPowerNumeral(0, 1);
            int PowerNumeralTargets = GetPowerNumeral(1, 1);
            int PowerNumeralDamage = GetPowerNumeral(2, 1);
            List<Card> charges = (List<Card>)FindCardsWhere((Card c) => c.Identifier == "ArcaneCharge" && c.IsInPlay);
            IEnumerator coroutine;
            if (charges.Count() > 0)
            {
                List<DestroyCardAction> dc = new List<DestroyCardAction>();
                coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.Identifier == "ArcaneCharge" && c.IsInPlay), true, dc, null, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (DidDestroyCard(dc))
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
                    ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(PowerNumeralReduce);
                    rdse.UntilStartOfNextTurn(TurnTaker);
                    rdse.TargetCriteria.IsSpecificCard = CharacterCard;
                    coroutine = GameController.AddStatusEffect(rdse, true, GetCardSource());
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
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), PowerNumeralDamage, DamageType.Radiant, PowerNumeralTargets, false, PowerNumeralTargets, cardSource: GetCardSource());
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
