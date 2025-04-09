using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using GuardiansOfTomorrow.TheReverseEngineer;

namespace GuardiansOfTomorrow.TheReverseEngineer
{
    class AugmentedChassisCardController : TheReverseEngineerCardController
    {
        public AugmentedChassisCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator FromTrashAction()
        {
            IEnumerator coroutine = GameController.GainHP(CharacterCard, 3, cardSource: GetCardSource());
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
            AddReduceDamageTrigger((Card c) => c == CharacterCard, 1);
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard, (DealDamageAction dd) => 1);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealSelfDamageOrDestroyThis, TriggerType.DestroySelf);
        }

        private IEnumerator DealSelfDamageOrDestroyThis(PhaseChangeAction pc)
        {
            List<Function> options = new List<Function>();
            options.Add(new Function(DecisionMaker, "{TheReverseEngineer} deals himself 2 fire damage", SelectionType.DealDamage, () => GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c == CharacterCard, 2, DamageType.Fire, cardSource: GetCardSource())));
            options.Add(new Function(DecisionMaker, "Destroy this card", SelectionType.DestroySelf, () => DestroyThisCardResponse(pc)));
            SelectFunctionDecision decision = new SelectFunctionDecision(GameController, DecisionMaker, options, false, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(decision);
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
