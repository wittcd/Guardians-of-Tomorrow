using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Nyctophobia
{
    class DeadlyMomentumCardController : CardController
    {
        public DeadlyMomentumCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsEnteredPlayThisTurn(new LinqCardCriteria((Card c) => c.Owner == TurnTaker, ""));
        }

        public override IEnumerator Play()
        {
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(GetNumberOfCardsPlayedThisTurn(TurnTakerController));
            idse.NumberOfUses = 1;
            idse.SourceCriteria.IsSpecificCard = CharacterCard;
            IEnumerator coroutine = AddStatusEffect(idse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectAndUsePowerEx(TurnTakerController.ToHero(), cardSource: GetCardSource());
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
