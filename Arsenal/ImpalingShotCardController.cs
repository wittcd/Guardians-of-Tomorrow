using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Arsenal
{
    public class ImpalingShotCardController : CardController
    {
        public ImpalingShotCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<SelectCardDecision> cardSelect = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Projectile, 1, false, 1, storedResultsDecisions: cardSelect, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectCard(cardSelect))
            {
                Card c = cardSelect.FirstOrDefault().SelectedCard;
                OnDealDamageStatusEffect oddse = new OnDealDamageStatusEffect(this.CardWithoutReplacements, "CancelDamage", "Prevent the next damage dealt by " + c.Title, new[] { TriggerType.WouldBeDealtDamage }, this.TurnTaker, this.Card, new int[] { });
                oddse.SourceCriteria.IsSpecificCard = c;
                oddse.NumberOfUses = 1;
                oddse.BeforeOrAfter = BeforeOrAfter.Before;

                coroutine = GameController.AddStatusEffectEx(oddse, true, GetCardSource());
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

        public IEnumerator CancelDamage(DealDamageAction dd, HeroTurnTaker htt, StatusEffect se, int[] powerNumerals = null)
        {
            IEnumerator coroutine = CancelActionEx(dd, isPreventEffect: true);
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
