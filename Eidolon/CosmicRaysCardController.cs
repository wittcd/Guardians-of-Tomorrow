﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class CosmicRaysCardController : AlterationCardController
    {
        public CosmicRaysCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            LinqCardCriteria linq = new LinqCardCriteria((Card c) => IsHero(c) && IsOngoing(c), "Hero Ongoing");
            IEnumerator coroutine = AlterationPlayedResponse(DamageType.Energy, GameController.SelectAndDestroyCards(DecisionMaker, linq, H - 1, cardSource: GetCardSource()));
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
