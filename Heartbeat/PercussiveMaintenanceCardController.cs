using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Heartbeat
{
    class PercussiveMaintenanceCardController : CardController
    {
        public PercussiveMaintenanceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.Identifier == "ArcaneCharge", "copies of Arcane Charge", false, false, "Copy of Arcane Charge", "Copies of Arcane Charge", false));
        }

        public override IEnumerator Play()
        {
            List<DealDamageAction> stored = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamage(CharacterCard, CharacterCard, 1, DamageType.Melee, false, false, false, null, stored, null, false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(stored))
            {
                //Get AC from trash then play card.

                coroutine = GameController.SelectAndMoveCard(DecisionMaker, (Card c) => c.Identifier == "ArcaneCharge" && c.Location == TurnTaker.Trash, TurnTaker.PlayArea, false, false, true, true, null, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.SelectAndPlayCardFromHand(DecisionMaker, false, cardSource: GetCardSource());
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
