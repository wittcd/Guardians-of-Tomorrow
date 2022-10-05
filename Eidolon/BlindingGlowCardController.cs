using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class BlindingGlowCardController : AlterationCardController
    {
        public BlindingGlowCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP();
        }

        public override IEnumerator Play()
        {
            List<Card> highestHero = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithHighestHitPoints(1, (Card c) => c.IsHeroCharacterCard, highestHero, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidFindCard(highestHero))
            {
                coroutine = AlterationPlayedResponse(DamageType.Radiant, GameController.SelectAndDiscardCards(FindTurnTakerController(highestHero.FirstOrDefault().Owner).ToHero(), 2, false, 2, cardSource: GetCardSource()));
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
