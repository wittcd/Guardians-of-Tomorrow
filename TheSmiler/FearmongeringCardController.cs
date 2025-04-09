using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheSmiler
{
    public class FearmongeringCardController : CardController
    {
        public FearmongeringCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //Destroy the environment target with the highest HP.\
            List<Card> highest = new List<Card>();
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.FindTargetWithHighestHitPoints(1, (Card c) => c.IsEnvironment, highest, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => highest.Contains(c), "environment"), false, storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            
            //If a card is destroyed this way, {TheSmiler} deals each non-villain target 1 psychic damage.
            if (DidDestroyCard(storedResults))
            {
                coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => !IsVillain(c), 3, DamageType.Psychic, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //Otherwise, increase damage dealt to hero targets by 1 until the start of the villain turn,
            //then play the top card of the villain deck.
            else
            {
                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
                idse.UntilStartOfNextTurn(TurnTaker);
                idse.TargetCriteria.IsHero = true;
                coroutine = GameController.AddStatusEffectEx(idse, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.PlayTopCard(DecisionMaker, TurnTakerController, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //Shuffle the villain trash into the villain deck.
            coroutine = GameController.ShuffleTrashIntoDeck(TurnTakerController, cardSource: GetCardSource());
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
