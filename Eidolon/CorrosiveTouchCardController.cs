using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class CorrosiveTouchCardController : AlterationCardController
    {
        public CorrosiveTouchCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            LinqCardCriteria linq = new LinqCardCriteria((Card c) => IsEquipment(c), "Equipment");
            IEnumerator coroutine = AlterationPlayedResponse(DamageType.Toxic, GameController.DestroyCards(DecisionMaker, linq, cardSource: GetCardSource()));
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
