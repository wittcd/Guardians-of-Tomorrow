using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace GuardiansOfTomorrow.Angler
{
    public class DeepSirenAnglerCharacterCardController : HeroCharacterCardController
    {
        public DeepSirenAnglerCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //{Angler} deals 1 target 1 sonic damage. Redirect the next damage dealt by each target dealt damage this way to {Angler}
            int powerNumeralTargets = GetPowerNumeral(0, 1);
            int powerNumeralDamage = GetPowerNumeral(1, 1);
            List<DealDamageAction> StoredResults = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), powerNumeralDamage, DamageType.Sonic, powerNumeralTargets, false, powerNumeralTargets, storedResultsDamage: StoredResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            foreach (DealDamageAction damage in StoredResults) 
            {
                if (damage.DidDealDamage)
                {
                    RedirectDamageStatusEffect rdse = new RedirectDamageStatusEffect();
                    rdse.SourceCriteria.IsSpecificCard = damage.Target;
                    rdse.NumberOfUses = 1;
                    rdse.RedirectTarget = CharacterCard;
                    coroutine = GameController.AddStatusEffectEx(rdse, true, GetCardSource());
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        // One hero may use a power
                        IEnumerator coroutine = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
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
                case 2:
                    {
                        // Destroy a target with 2 or less HP. If a target is destroyed this way, one hero gains 2 HP.
                        List<DestroyCardAction> destroys = new List<DestroyCardAction>();
                        IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsTarget && c.HitPoints <= 2, "target with 2 or less HP", false, false, "target with 2 or less HP", "targets with 2 or less HP"), false, destroys, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (DidDestroyCard(destroys))
                        {
                            coroutine = GameController.SelectAndGainHP(DecisionMaker, 2, false, (Card c) => IsHeroCharacterCard(c), cardSource: GetCardSource());
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
