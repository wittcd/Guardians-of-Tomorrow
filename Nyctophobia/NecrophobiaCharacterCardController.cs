using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Nyctophobia
{
    internal class NecrophobiaCharacterCardController: HeroCharacterCardController
    {
        public NecrophobiaCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //{Nyctophobia} deals 1 target 1 infernal damage. If a target is destroyed this way
            //select a hero deck and reveal cards from that deck until a target or ongoing card is revealed.
            //Put the revealed card into play and shuffle the remaining cards back into the deck.
            int PowerNumeralTargets = GetPowerNumeral(0, 1);
            int PowerNumeralDamage = GetPowerNumeral(1, 1);
            List<DealDamageAction> ddlist = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), PowerNumeralDamage, DamageType.Infernal, PowerNumeralTargets, false, PowerNumeralTargets, storedResultsDamage: ddlist, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            bool destroyed = false;
            foreach (DealDamageAction dd in ddlist)
            {
                if (!destroyed && dd.DidDestroyTarget)
                {
                    destroyed = true;
                    coroutine = SelectHeroAndRevealUntilOngoingOrTarget();
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

        private IEnumerator SelectHeroAndRevealUntilOngoingOrTarget()
        {
            List<SelectLocationDecision> selectLocList = new List<SelectLocationDecision>();
            IEnumerable<LocationChoice> options = FindLocationsWhere((Location l) => l.IsHero && l.IsDeck).Select((Location L) => new LocationChoice(L));
            IEnumerator coroutine = GameController.SelectLocation(DecisionMaker, options, SelectionType.RevealCardsFromDeck, selectLocList, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            
            if (DidSelectLocation(selectLocList)) {
                Location d = selectLocList.FirstOrDefault().SelectedLocation.Location;
                coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, d, false, true, false, new LinqCardCriteria((Card c) => c.IsTarget || IsOngoing(c), "ongoing or target"), 1);
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
            /*
            "One player may draw a card.",
            "Destroy an environment card.",
            "Put a target or ongoing card from a hero trash into play."
            */

            switch (index)
            {
                case 0:
                    {
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
                case 1:
                    {
                        IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), false, cardSource: GetCardSource());
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
                        IEnumerator coroutine = GameController.SelectHeroToSelectTargetAndDealDamage(DecisionMaker, 1, DamageType.Infernal, cardSource: GetCardSource());
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
            }
        }
    }
}
