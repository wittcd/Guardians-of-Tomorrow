using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Heartbeat
{
    class TotalDischargeCardController : CardController
    {
        public TotalDischargeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.Identifier == "ArcaneCharge", "copies of Arcane Charge", false, false, "Copy of Arcane Charge", "Copies of Arcane Charge", false));
        }

        public override IEnumerator Play()
        {
            List<DestroyCardAction> stored = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.Identifier == "ArcaneCharge" && c.IsInPlay), false, stored, null, null, SelectionType.DestroyCard, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            int d = GetNumberOfCardsDestroyed(stored);

            List<DealDamageAction> ddlist = new List<DealDamageAction>();
            ddlist.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, d, DamageType.Melee));
            ddlist.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, d, DamageType.Radiant));

            coroutine = SelectTargetAndDealMultipleInstancesOfDamage(ddlist);
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
