using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Heartbeat
{
    class DeadlyDanceCardController : CardController
    {
        public DeadlyDanceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.DamageType == DamageType.Melee, 1);
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cepa) => cepa.CardEnteringPlay.Identifier == "ArcaneCharge", (CardEntersPlayAction cepa) => GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Melee, 1, false, 1, cardSource: GetCardSource()), TriggerType.DealDamage, TriggerTiming.After);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Melee, 3, false, 0, cardSource: GetCardSource());
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
