using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Cathain
{
    public class LightningStrikeCardController : CardController
    {
        public LightningStrikeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"{Cathain} deals 1 target 4 lightning damage, then deals each other non-hero target 1 sonic damage.
            List<SelectCardDecision> selected = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 4, DamageType.Lightning, 1, false, 1, storedResultsDecisions: selected, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => !IsHeroTarget(c) && !SelectedContains(selected, c), 1, DamageType.Sonic, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private bool SelectedContains(List<SelectCardDecision> selected, Card c)
        {
            foreach (SelectCardDecision s in selected)
            {
                if (c == s.SelectedCard)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
