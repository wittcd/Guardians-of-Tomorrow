using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using GuardiansOfTomorrow.TheReverseEngineer;

namespace GuardiansOfTomorrow.TheReverseEngineer
{
    class TweakedCalibrationCardController : TheReverseEngineerCardController
    {
        public TweakedCalibrationCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator FromTrashAction()
        {
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(2);
            idse.SourceCriteria.IsSpecificCard = CharacterCard;
            idse.NumberOfUses = 1;
            IEnumerator coroutine = GameController.AddStatusEffect(idse, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = base.Play();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = SelectAndDiscardCards(TurnTakerController.ToHero(), 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = TheReverseEngineerExtensions.PlayTopCardOfTrash(this, TurnTakerController, GameController);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<UsePowerDecision> store = new List<UsePowerDecision>();
            coroutine = GameController.SelectAndUsePower(DecisionMaker, storedResults: store, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (store.FirstOrDefault() != null && store.FirstOrDefault().SelectedPower != null)
            {
                coroutine = DealDamage(CharacterCard, CharacterCard, 2, DamageType.Fire, cardSource: GetCardSource());
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
