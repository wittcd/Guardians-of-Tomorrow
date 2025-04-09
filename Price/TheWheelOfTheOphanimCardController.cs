using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.Price
{
    class TheWheelOfTheOphanimCardController : CardController
    {
        public TheWheelOfTheOphanimCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.DamageType == DamageType.Radiant, 1);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine = RevealCardsFromTopOfDeck_DetermineTheirLocationEx(DecisionMaker, TurnTakerController, TurnTaker.Deck, new MoveCardDestination(TurnTaker.Trash), new MoveCardDestination(TurnTaker.Deck), 2, 1, 1);
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
