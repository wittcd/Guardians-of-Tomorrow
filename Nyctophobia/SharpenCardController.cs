using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Nyctophobia
{
    internal class SharpenCardController : CardController
    {
        public SharpenCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(2);
            idse.SourceCriteria.IsSpecificCard = CharacterCard;
            idse.NumberOfUses = 1;

            coroutine = GameController.AddStatusEffectEx(idse, true, GetCardSource());
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
