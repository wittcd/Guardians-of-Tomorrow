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
    public class RestlessSpiritCardController : CardController
    {
        public RestlessSpiritCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DealDamageAndOptionalHandDiscard(), new TriggerType[] { TriggerType.DealDamage, TriggerType.DiscardCard, TriggerType.DestroyCard });
        }

        private IEnumerator DealDamageAndOptionalHandDiscard()
        {
            List<TurnTaker> cluelessHero = new List<TurnTaker>();
            IEnumerator coroutine = FindHeroWithFewestCardsInPlayArea(cluelessHero, 1, 1, null, new LinqCardCriteria((Card c) => c.IsClue, "clue"));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (cluelessHero.FirstOrDefault() != null)
            {
                List<DealDamageAction> storedResults = new List<DealDamageAction>();
                //coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), H - 2, DamageType.Projectile, 1, false, 1, additionalCriteria: (Card c) => c.Owner == cluelessHero.FirstOrDefault() && IsHeroCharacterCard(c));
                List<Card> chosenCC = new List<Card>();
                coroutine = FindCharacterCardToTakeDamage(cluelessHero.FirstOrDefault(), chosenCC, Card, H, DamageType.Projectile);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (chosenCC.FirstOrDefault() != null)
                {
                    coroutine = DealDamage(base.Card, chosenCC.FirstOrDefault(), H, DamageType.Projectile, storedResults: storedResults, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                List<DiscardCardAction> discards = new List<DiscardCardAction>();
                List<YesNoCardDecision> yn = new List<YesNoCardDecision>();
                coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DiscardHand, Card, null, yn, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidPlayerAnswerYes(yn))
                {
                    coroutine = GameController.DiscardHand(FindHeroTurnTakerController(cluelessHero.FirstOrDefault().ToHero()), true, discards, cluelessHero.FirstOrDefault(), GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                if (DidDiscardCards(discards))
                {
                    coroutine = GameController.DrawCards(FindHeroTurnTakerController(cluelessHero.FirstOrDefault().ToHero()), 3, false, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.IsClue, "clue"), 1);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = DestroyThisCardResponse(null);
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
