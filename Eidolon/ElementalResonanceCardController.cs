using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class ElementalResonanceCardController : CardController
    {

        private const string FirstDamageFromHeroThisTurn = "FirstDamageFromHeroThisTurn";
        public ElementalResonanceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstDamageFromHeroThisTurn, "Eidolon has been dealt damage by a hero character this turn.", "Eidolon has not been dealt damage by a hero character this turn.");
        }

        public override void AddTriggers()
        {
            AddTrigger((DealDamageAction dd) => !HasBeenSetToTrueThisTurn(FirstDamageFromHeroThisTurn) && IsHeroCharacterCard(dd.DamageSource.Card) && dd.DidDealDamage && dd.Target == base.CharacterCard, DealDamageThenForceConvertResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageThenForceConvertResponse(DealDamageAction dd)
        {
            SetCardPropertyToTrueIfRealAction(FirstDamageFromHeroThisTurn);
            IEnumerator coroutine = DealDamage(base.CharacterCard, dd.DamageSource.Card, 2, dd.DamageType, false, false, true, null, null, null, false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            ChangeDamageTypeStatusEffect cdtse = new ChangeDamageTypeStatusEffect(dd.DamageType);
            cdtse.SourceCriteria.IsSpecificCard = dd.DamageSource.Card;
            cdtse.UntilStartOfNextTurn(dd.DamageSource.Card.Owner);
            coroutine = AddStatusEffect(cdtse);
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
