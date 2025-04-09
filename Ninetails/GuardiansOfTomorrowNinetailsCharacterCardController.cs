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
            //may discard, if so 1 damage to 9 targets otherwise 1 damage to 3 targets (up to)
            int powerNumeralTargetsDisc = GetPowerNumeral(0, 9);
            int powerNumeralDamageDisc = GetPowerNumeral(1, 1);
            int powerNumeralTargetNot = GetPowerNumeral(2, 3);
            int powerNumeralDamageNot = GetPowerNumeral(3, 1);
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.SelectAndDiscardCard(DecisionMaker, true, null, storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidDiscardCards(storedResults))
            {
                DamageSource ds = new DamageSource(base.GameController, base.CharacterCard);
                coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, ds, powerNumeralDamageDisc, DamageType.Fire, powerNumeralTargetsDisc, optional: false, 0, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, GetCardSource());
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
                DamageSource ds = new DamageSource(base.GameController, base.CharacterCard);
                coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, ds, powerNumeralDamageNot, DamageType.Fire, powerNumeralTargetNot, optional: false, 0, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, GetCardSource());
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