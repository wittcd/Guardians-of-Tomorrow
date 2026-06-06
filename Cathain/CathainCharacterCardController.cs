using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace GuardiansOfTomorrow.Cathain
{
    public class CathainCharacterCardController : HeroCharacterCardController
    {
        public CathainCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeralCards = GetPowerNumeral(0, 1);
            //If you have a Locus in play,
            List<Card> loci = FindCardsWhere((Card c) => GameController.DoesCardContainKeyword(c, "locus") && c.Owner == TurnTaker && c.IsInPlayAndNotUnderCard).ToList();
            if (loci.Count > 0)
            {
                //reveal cards from your deck until 1 card that shares a keyword with a Locus in play is revealed. Put that card in your hand and shuffle the other revealed cards into your deck.
                List<String> locusKeywords = new List<String>();
                foreach (Card l in loci)
                {
                    locusKeywords.AddRange(l.GetKeywords());
                }

                IEnumerator coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, TurnTaker.Deck, false, false, true, new LinqCardCriteria((Card c) => getKeywordOverlap(locusKeywords, c) && !GameController.DoesCardContainKeyword(c, "locus"), "that share a keyword with one of your Loci", false, true), powerNumeralCards, revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards);
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
                //Otherwise, search your deck or trash for a Locus and put it into your hand.If you searched your deck, shuffle it.
                List<Function> options = new List<Function>();
                
                List<MoveCardDestination> destinations = new List<MoveCardDestination>();
                destinations.Add(new MoveCardDestination(TurnTaker.ToHero().Hand));
                
                options.Add(new Function(DecisionMaker, "search your deck", SelectionType.SearchDeck, () => GameController.SelectCardFromLocationAndMoveIt(DecisionMaker, TurnTaker.Deck, new LinqCardCriteria((Card c) => GameController.DoesCardContainKeyword(c, "locus"), "locus"), destinations, shuffleAfterwards: true, cardSource: GetCardSource())));
                options.Add(new Function(DecisionMaker, "search your trash", SelectionType.SearchDeck, () => GameController.SelectCardFromLocationAndMoveIt(DecisionMaker, TurnTaker.Trash, new LinqCardCriteria((Card c) => GameController.DoesCardContainKeyword(c, "locus"), "locus"), destinations, shuffleAfterwards: false, cardSource: GetCardSource())));
                SelectFunctionDecision sfd = new SelectFunctionDecision(GameController, DecisionMaker, options, false, cardSource: GetCardSource());
                IEnumerator coroutine = GameController.SelectAndPerformFunction(sfd);
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        // One player may play a card
                        IEnumerator coroutine = GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        // One player may draw a card
                        IEnumerator coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 2:
                    {
                        // One hero may deal themselves 1 irreducible infernal damage. If they take damage this way, they may deal 1 other target 3 cold, fire, lightning, or melee damage.
                        List<DealDamageAction> damages = new List<DealDamageAction>();
                        List<SelectCardDecision> selected = new List<SelectCardDecision>();
                        IEnumerator coroutine = GameController.SelectTargetsToDealDamageToSelf(DecisionMaker, 1, DamageType.Infernal, 1, false, 0, isIrreducible: true, storedResultsDecisions: selected, additionalCriteria: (Card c) => IsHeroCharacterCard(c), storedResultsDamage: damages, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (selected.Count() >= 1 && DidDealDamage(damages, selected.FirstOrDefault().SelectedCard))
                        {
                            List<SelectDamageTypeDecision> selectedDT = new List<SelectDamageTypeDecision>();
                            coroutine = GameController.SelectDamageType(DecisionMaker, selectedDT, new DamageType[] { DamageType.Cold, DamageType.Fire, DamageType.Lightning, DamageType.Melee }, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            DamageType dt = selectedDT.FirstOrDefault().SelectedDamageType.Value;

                            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, selected.FirstOrDefault().SelectedCard), 3, dt, 1, false, 1, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
            }
        }

        private bool getKeywordOverlap(IEnumerable<String> keywords, Card c)
        {
            List<String> k = c.GetKeywords().ToList();
            foreach (String s in k)
            {
                if (keywords.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
