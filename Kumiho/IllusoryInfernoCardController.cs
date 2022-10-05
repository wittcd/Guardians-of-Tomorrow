using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Kumiho
{
    class IllusoryInfernoCardController : CardController
    {
        public IllusoryInfernoCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = GameController.YesNoDoAction_ManyPlayers((HeroTurnTakerController httc) => !httc.IsIncapacitatedOrOutOfGame, (HeroTurnTakerController httc) => new YesNoDecision(GameController, httc, SelectionType.DiscardCard, cardSource: GetCardSource()), DealDamageBasedOnDiscardsResponse, JustDealFullDamageResponse, null, SelectionType.DiscardCard, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }


        private IEnumerator DealDamageBasedOnDiscardsResponse(HeroTurnTakerController httc, YesNoDecision ynd)
        {
            List<DiscardCardAction> discardList = new List<DiscardCardAction>();
            IEnumerator coroutine = SelectAndDiscardCards(httc, 2, false, null, discardList, false, null, null, null, SelectionType.DiscardCard, base.TurnTaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidDiscardCards(discardList, 2))
            {
                coroutine = DealDamage(httc.CharacterCard, httc.CharacterCard, 1, DamageType.Psychic, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                coroutine = DealDamage(base.CharacterCard, httc.CharacterCard, 2, DamageType.Fire, cardSource: GetCardSource());
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

        private IEnumerator JustDealFullDamageResponse(HeroTurnTakerController httc, YesNoDecision ynd)
        {
            IEnumerator coroutine = DealDamage(base.CharacterCard, httc.CharacterCard, 2, DamageType.Fire, cardSource: GetCardSource());
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
