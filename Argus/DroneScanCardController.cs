using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace GuardiansOfTomorrow.Argus
{
    class DroneScanCardController : CardController
    {
        public DroneScanCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = DrawCards(DecisionMaker, 2, false, false, null, true, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            int amount = FindCardsWhere((Card c) => IsEquipment(c) && c.Owner == base.TurnTaker && c.IsInPlay, false, GetCardSource(), false).Count();
            if (amount >= 3) 
            {
                coroutine = DrawCards(DecisionMaker, 2, false, false, null, true, null);
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
