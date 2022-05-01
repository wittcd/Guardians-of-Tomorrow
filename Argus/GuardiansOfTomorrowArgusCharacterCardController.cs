using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Argus
{
    public class GuardiansOfTomorrowArgusCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public GuardiansOfTomorrowArgusCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //deal 2 self-psychic damage.
            List<DealDamageAction> storedDamage = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Psychic, false, false, false, null, storedDamage, null, false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //if damage was dealt, draw, then operate, then deal 1 target 2 projectile
            if (DidDealDamage(storedDamage, base.CharacterCard, null)) 
            {
                coroutine = DrawCard(base.TurnTaker.ToHero());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                LinqCardCriteria isCamdrone = new LinqCardCriteria((Card c) => c.DoKeywordsContain("camdrone") && c.IsInPlay, "camdrone in play");
                coroutine = GameController.SelectAndActivateAbility(DecisionMaker, "operate", isCamdrone, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), 2, DamageType.Projectile, 1, false, 1, cardSource: GetCardSource());
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
                        //one player may use a power.
                        IEnumerator coroutine = GameController.SelectHeroToUsePower(DecisionMaker, false, true, cardSource: GetCardSource());
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
                        //destroy env card
                        IEnumerator coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment card", useCardsSuffix: false), optional: false, null, null, GetCardSource());
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
                        //one player deals 2 self psychic damage, if they do they may draw and one player may play
                        List<DealDamageAction> storedDamage = new List<DealDamageAction>();
                        IEnumerator coroutine = GameController.SelectTargetsToDealDamageToSelf(DecisionMaker, 2, DamageType.Psychic, 1, false, 1, false, false, false, (Card c) => c.IsHeroCharacterCard, null, storedDamage, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (storedDamage.FirstOrDefault() != null && DidDealDamage(storedDamage, storedDamage.FirstOrDefault().OriginalTarget, null)) 
                        {
                            coroutine = GameController.DrawCard(storedDamage.FirstOrDefault().OriginalTarget.Owner.ToHero(), true, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                            coroutine = GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: GetCardSource());
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
    }
}