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
    public class MeltArmorCardController : CardController
    {
        public MeltArmorCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //{Cathain} deals 1 target 4 fire damage. Increase damage dealt to that target by 1 until the start of your turn.
            List<SelectCardDecision> chosentarget = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 4, DamageType.Fire, 1, false, 1, storedResultsDecisions: chosentarget, cardSource: GetCardSource());
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
                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
                idse.TargetCriteria.IsSpecificCard = target;
                idse.UntilStartOfNextTurn(TurnTaker);
                coroutine = GameController.AddStatusEffectEx(idse, true, GetCardSource());
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
