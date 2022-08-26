using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheHatter
{
    class PlayingWithLivesCardController : CardController
    {
        public PlayingWithLivesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        { }

        public override IEnumerator Play()
        {
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamage((Card c) => c == base.CharacterCard, (Card c) => c.DoKeywordsContain("thrall"), (Card c) => 5, DamageType.Sonic, storedResults:storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            bool destroyed = false;
            foreach (DealDamageAction dd in storedResults)
            {
                if (dd.DidDestroyTarget)
                {
                    destroyed = true;
                }
            }

            if (destroyed)
            {
                coroutine = DealDamage((Card c) => c == base.CharacterCard, (Card c) => c.IsHero, (Card c) => H, DamageType.Psychic);
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
