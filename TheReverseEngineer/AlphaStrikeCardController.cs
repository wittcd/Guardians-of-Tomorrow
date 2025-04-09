using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using GuardiansOfTomorrow.TheReverseEngineer;

namespace GuardiansOfTomorrow.TheReverseEngineer
{
    class AlphaStrikeCardController : TheReverseEngineerCardController
    {
        public AlphaStrikeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator FromTrashAction()
        {
            IEnumerator coroutine = GameController.SelectAndUsePower(DecisionMaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            AddTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, UseAnyNumberOfPowersAndTakeDamage, TriggerType.UsePower));
        }

        private IEnumerator UseAnyNumberOfPowersAndTakeDamage(PhaseChangeAction pc)
        {
            List<UsePowerDecision> store = new List<UsePowerDecision>();
            IEnumerator coroutine = GameController.SelectAndUsePower(DecisionMaker, true, null, 10, true, store, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            int count = 0;

            foreach (UsePowerDecision use in store)
            {
                if (use != null && use.SelectedPower != null)
                {
                    count += 1;
                }
            }
            coroutine = DealDamage(CharacterCard, CharacterCard, count, DamageType.Fire, cardSource: GetCardSource());
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
