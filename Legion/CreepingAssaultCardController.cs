using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Legion
{
    class CreepingAssaultCardController : CardController
    {
        public CreepingAssaultCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP();
        }

        public override IEnumerator Play()
        {
            List<DealDamageAction> stored = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => IsHeroCharacterCard(c), (Card c) => H, DamageType.Projectile, storedResults: stored, selectTargetEvenIfCannotDealDamage: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card target = stored.FirstOrDefault().OriginalTarget;
            coroutine = GameController.SelectAndDiscardCards(FindTurnTakerController(target.Owner).ToHero(), 2, false, 2, cardSource: GetCardSource());
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
