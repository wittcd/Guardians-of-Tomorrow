﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Arsenal
{
    class UchideNoKozuchiCardController : ArsenalRelicCardController
    {
        public UchideNoKozuchiCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            IEnumerator coroutine = ReturnAllButTwoRelicsToHand();
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
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.DamageType == DamageType.Melee, (DealDamageAction dd) => 1, false);
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.DamageType == DamageType.Melee && GetNumberOfMeleeDealtThisTurn() <= 1, (DealDamageAction dd) => GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource()), TriggerType.DrawCard, TriggerTiming.After);
        }
    }
}
