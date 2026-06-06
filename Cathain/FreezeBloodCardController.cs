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
    public class FreezeBloodCardController  : CardController
    {
        public FreezeBloodCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //{Cathain} deals 1 target 4 cold damage. Reduce damage dealt by that target by 1 until the start of your turn.
            List<SelectCardDecision> chosentarget = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 4, DamageType.Cold, 1, false, 1, storedResultsDecisions: chosentarget, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (chosentarget.Count > 0)
            {
                Card target = chosentarget.FirstOrDefault().SelectedCard;
                ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                rdse.SourceCriteria.IsSpecificCard = target;
                rdse.UntilStartOfNextTurn(TurnTaker);
                coroutine = GameController.AddStatusEffectEx(rdse, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //{Cathain} or one of your Loci deals itself 3 irreducible infernal damage."
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 3, DamageType.Infernal, 1, false, 1, additionalCriteria: (Card c) => c == CharacterCard, cardSource: GetCardSource());
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
