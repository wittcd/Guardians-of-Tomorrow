using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class CrushingGripCardController : CardController
    {
        public CrushingGripCardController(Card card, TurnTakerController turnTakerController)
                    : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DealDamageAction> damages = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => H, DamageType.Melee, storedResults: damages);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(damages))
            {
                CannotDealDamageStatusEffect cddse = new CannotDealDamageStatusEffect();
                cddse.SourceCriteria.IsSpecificCard = damages.FirstOrDefault().Target;
                cddse.UntilStartOfNextTurn(TurnTaker);
                coroutine = GameController.AddStatusEffectEx(cddse, true, GetCardSource());
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
