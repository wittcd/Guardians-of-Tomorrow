using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class EldritchConduitCardController : ChanceCardController
    {
        public EldritchConduitCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<SelectDamageTypeDecision> resultType = new List<SelectDamageTypeDecision>();
            IEnumerator coroutine = GameController.SelectDamageType(DecisionMaker, resultType, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            ImmuneToDamageStatusEffect itdse = new ImmuneToDamageStatusEffect();
            itdse.UntilEndOfNextTurn(base.TurnTaker);
            itdse.DamageTypeCriteria.AddType(resultType.FirstOrDefault().SelectedDamageType.Value);
            itdse.TargetCriteria.IsSpecificCard = base.CharacterCard;

            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.UntilEndOfNextTurn(TurnTaker);
            idse.DamageTypeCriteria.AddType(resultType.FirstOrDefault().SelectedDamageType.Value);
            idse.SourceCriteria.IsSpecificCard = base.CharacterCard;

            coroutine = GameController.AddStatusEffect(itdse, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.AddStatusEffect(idse, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), 1, resultType.FirstOrDefault().SelectedDamageType.Value, 3, false, 0, cardSource: GetCardSource());
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
