using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using Handelabra.Sentinels.Engine;

namespace GuardiansOfTomorrow.TheSmiler
{
    public class SacrificialVictimCardController : CardController
    {
        public SacrificialVictimCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<Card> lowestHP = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetsWithLowestHitPoints(1, 1, (Card c) => !c.IsCharacter, lowestHP, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

            if(DidFindCard(lowestHP))
            {
                coroutine = GameController.MoveCard(TurnTakerController, Card, lowestHP.FirstOrDefault().NextToLocation, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.SendMessageAction("Sacrificial Victim puts itself next to " + GetCardThisCardIsNextTo().Title, Priority.High, cardSource: GetCardSource(), new Card[] { Card });
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
                coroutine = GameController.PlayTopCard(DecisionMaker, TurnTakerController, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.DestroyCard(DecisionMaker, Card, cardSource: GetCardSource());
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

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => StartOfTurnResponse(), new TriggerType[] { TriggerType.DestroySelf, TriggerType.DealDamage, TriggerType.GainHP });
            AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();
        }

        private IEnumerator StartOfTurnResponse()
        {
            Card nextToCard = GetCardThisCardIsNextTo();

            //At the start of the villain turn, {TheSmiler} deals the target next to this card 3 irreducible infernal damage.
            List<DealDamageAction> damages = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c == nextToCard, 3, DamageType.Infernal, true, storedResults: damages, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If it is destroyed this way, {TheSmiler} gains 5 HP and deals each non-villain target 3 irreducible infernal damage, then destroy this card
            if (damages.FirstOrDefault() != null && damages.FirstOrDefault().DidDestroyTarget && damages.FirstOrDefault().Target == nextToCard)
            {
                coroutine = GameController.GainHPEx(CharacterCard, 5, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => !IsVillain(c), 3, DamageType.Infernal, true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.DestroyCard(DecisionMaker, Card, cardSource: GetCardSource());
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
