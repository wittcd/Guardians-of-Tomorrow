using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Ninetails
{
    public class GuardiansOfTomorrowNinetailsCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public GuardiansOfTomorrowNinetailsCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //discard up to 3 cards. Ninetails deals up to X targets 1 fire damage each where X is 3 + twice the number of cards discarded this way
            int powerNumeralDiscard = GetPowerNumeral(0, 3);
            int powerNumeralDamage = GetPowerNumeral(1, 1);
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = SelectAndDiscardCards(DecisionMaker, powerNumeralDiscard, optional: false, 0, storedResults, false, null, null, null, selectionType: SelectionType.DiscardCard, responsibleTurnTaker: base.TurnTaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            int numberOfCardsDiscarded = GetNumberOfCardsDiscarded(storedResults);
            int x = 3 + (2 * numberOfCardsDiscarded);
            DamageSource ds = new DamageSource(base.GameController, base.CharacterCard);
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, ds, powerNumeralDamage, DamageType.Fire, x, optional: false, 0, isIrreducible: false, allowAutoDecide:false, autoDecide: false, null, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //draw card
                        GameController gameController2 = base.GameController;
                        HeroTurnTakerController decisionMaker = DecisionMaker;
                        CardSource cardSource = GetCardSource();
                        IEnumerator coroutine3 = gameController2.SelectHeroToDrawCard(decisionMaker, optionalSelectHero: false, optionalDrawCard: true, allowAutoDecideHero: false, null, null, null, cardSource);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
                case 1:
                    {
                        //play card
                        IEnumerator coroutine2 = SelectHeroToPlayCard(DecisionMaker);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //irreduciblize damage
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.SelectTargetNoDamage, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay, "targets in play", useCardsSuffix: false), storedResults, optional: false, allowAutoDecide: false, null, includeRealCardsOnly: true, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        Card card = (from d in storedResults
                                     where d.Completed
                                     select d.SelectedCard).FirstOrDefault();
                        if (card != null)
                        {
                            IEnumerator coroutine2 = AddMakeDamageIrreducibleStatusEffect(card, base.TurnTaker, Phase.Start);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine2);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine2);
                            }
                        }
                        break;
                    }
            }
        }
    }
}