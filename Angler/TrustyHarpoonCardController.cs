using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Angler
{
    public class TrustyHarpoonCardController : CardController
    {
        public TrustyHarpoonCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<DealDamageAction> stored = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 3, DamageType.Projectile, 1, false, 1, storedResultsDamage: stored, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(stored) && !stored.FirstOrDefault().DidDestroyTarget && !stored.FirstOrDefault().Target.IsCharacter)
            {
                RedirectDamageStatusEffect rdse = new RedirectDamageStatusEffect();
                rdse.SourceCriteria.IsSpecificCard = stored.FirstOrDefault().Target;
                rdse.RedirectTarget = CharacterCard;
                rdse.UntilStartOfNextTurn(TurnTaker);
                coroutine = GameController.AddStatusEffect(rdse, true, GetCardSource());
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
