using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Cathain
{
    public class SiphonElementsCardController : CardController
    {
        public SiphonElementsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageType == DamageType.Infernal && dd.Target == CharacterCard, IncreaseNextNonInfernal, TriggerType.IncreaseDamage, TriggerTiming.After);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<SelectCardDecision> selected = new List<SelectCardDecision>();
            SelectCardDecision scd = new SelectCardDecision(GameController, DecisionMaker, SelectionType.DestroyCard, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && GameController.DoesCardContainKeyword(c, "locus") && c.Owner == TurnTaker), cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectCardAndDoAction(scd, DestroyAndHealBasedOnHP);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DestroyAndHealBasedOnHP(SelectCardDecision scd)
        {
            if (scd.SelectedCard != null && scd.SelectedCard.HitPoints.HasValue)
            {
                int prev_HP = scd.SelectedCard.HitPoints.Value;
                IEnumerator coroutine = GameController.DestroyCard(DecisionMaker, scd.SelectedCard, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.GainHPEx(CharacterCard, prev_HP / 2 + prev_HP % 2, cardSource: GetCardSource());
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
                IEnumerator coroutine = GameController.DoNothing();
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

        private IEnumerator IncreaseNextNonInfernal(DealDamageAction dd)
        {
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.SourceCriteria.IsSpecificCard = CharacterCard;
            /*idse.DamageTypeCriteria.AddType(DamageType.Cold);
            idse.DamageTypeCriteria.AddType(DamageType.Energy);
            idse.DamageTypeCriteria.AddType(DamageType.Fire);
            idse.DamageTypeCriteria.AddType(DamageType.Lightning);
            idse.DamageTypeCriteria.AddType(DamageType.Melee);
            idse.DamageTypeCriteria.AddType(DamageType.Projectile);
            idse.DamageTypeCriteria.AddType(DamageType.Psychic);
            idse.DamageTypeCriteria.AddType(DamageType.Radiant);
            idse.DamageTypeCriteria.AddType(DamageType.Sonic);
            idse.DamageTypeCriteria.AddType(DamageType.Toxic);*/
            idse.DamageTypeCriteria.AddType(DamageType.Infernal);
            idse.DamageTypeCriteria.invertTypes = true;
            idse.NumberOfUses = 1;

            IEnumerator coroutine = GameController.AddStatusEffectEx(idse, true, GetCardSource());
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
