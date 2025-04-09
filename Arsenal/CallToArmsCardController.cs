using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Arsenal
{
    public class CallToArmsCardController : CardController
    {
        public CallToArmsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }


        public override IEnumerator Play()
        {
            IEnumerator coroutine = base.GameController.SelectCardFromLocationAndMoveIt(base.HeroTurnTakerController, base.HeroTurnTakerController.HeroTurnTaker.Deck, new LinqCardCriteria((Card c) => c.IsRelic, "relic"), new MoveCardDestination[2]
                {
                new MoveCardDestination(base.HeroTurnTakerController.HeroTurnTaker.PlayArea),
                new MoveCardDestination(base.HeroTurnTakerController.HeroTurnTaker.Hand)
                }, isPutIntoPlay: true, playIfMovingToPlayArea: true, shuffleAfterwards: true, optional: false, null, flipFaceDown: false, showOutput: false, null, isDiscardIfMovingtoTrash: false, allowAutoDecide: false, null, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            DamageType[] types = { DamageType.Melee, DamageType.Projectile };
            List<SelectDamageTypeDecision> selectedType = new List<SelectDamageTypeDecision>();
            coroutine = GameController.SelectDamageType(DecisionMaker, selectedType, types, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, selectedType.FirstOrDefault().SelectedDamageType.Value, 1, false, 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
