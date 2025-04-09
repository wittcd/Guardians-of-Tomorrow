 using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Ninetails
{
    public class NinetailsCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public NinetailsCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //next time you would be dealt 2 or less damage, redirect it to another target
            RedirectDamageStatusEffect redirectDamageStatusEffect = new RedirectDamageStatusEffect();
            redirectDamageStatusEffect.TargetCriteria.IsSpecificCard = base.CharacterCard;
            redirectDamageStatusEffect.RedirectableTargets.IsTarget = true;
            redirectDamageStatusEffect.NumberOfUses = 1;
            redirectDamageStatusEffect.IsOptional = false;
            redirectDamageStatusEffect.DamageAmountCriteria.LessThan = 3;
            if (!base.GameController.StatusEffectControllers.Any((StatusEffectController sec) => sec.StatusEffect.CardSource == base.Card))
            {
                IEnumerator coroutine = AddStatusEffect(redirectDamageStatusEffect);
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
                        //use power
                        IEnumerator coroutine = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, GetCardSource());
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
                        //destroy ong
                        IEnumerator coroutine = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c), "ongoing"), optional: false, null, null, GetCardSource());
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
                        RedirectDamageStatusEffect redirectDamageStatusEffect = new RedirectDamageStatusEffect();
                        redirectDamageStatusEffect.TargetCriteria.IsHero = true;
                        redirectDamageStatusEffect.TargetCriteria.IsTarget = true;
                        redirectDamageStatusEffect.RedirectableTargets.IsTarget = true;
                        redirectDamageStatusEffect.NumberOfUses = 1;
                        if (!base.GameController.StatusEffectControllers.Any((StatusEffectController sec) => sec.StatusEffect.CardSource == base.Card))
                        {
                            IEnumerator coroutine = AddStatusEffect(redirectDamageStatusEffect);
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