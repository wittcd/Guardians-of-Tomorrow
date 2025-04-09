using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Nyctophobia
{
    class HideInPlainSightCardController : CardController
    {
        public HideInPlainSightCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DiscardCardAction> storedDiscards = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.SelectAndDiscardCards(TurnTakerController.ToHero(), 3, false, 0, storedDiscards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            int numdisc = GetNumberOfCardsDiscarded(storedDiscards);
            if (numdisc > 0)
            {
                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(numdisc);
                idse.SourceCriteria.IsSpecificCard = CharacterCard;
                idse.NumberOfUses = 1;
                ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(numdisc);
                rdse.TargetCriteria.IsSpecificCard = CharacterCard;
                rdse.UntilStartOfNextTurn(TurnTaker);

                coroutine = GameController.AddStatusEffectEx(rdse, false, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.AddStatusEffectEx(idse, false, GetCardSource());
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
