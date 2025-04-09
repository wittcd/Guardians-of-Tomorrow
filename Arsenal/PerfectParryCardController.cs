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
    public class PerfectParryCardController : CardController
    {
        private bool saidYes = false;

        public PerfectParryCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == CharacterCard && dd.DamageSource.IsCard && !IsHero(dd.DamageSource.Card) && dd.DamageSource.Card.IsTarget, PreventAndCounter, TriggerType.CancelAction, TriggerTiming.Before, ActionDescription.DamageTaken, false, true, true, false, null, null, false, false);
        }

        private IEnumerator PreventAndCounter(DealDamageAction dd)
        {
            List<YesNoCardDecision> decisions = new List<YesNoCardDecision>();
            Card source = dd.DamageSource.Card;
            if (dd.IsPretend)
            {
                IEnumerator coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.PreventDamage, Card, null, decisions, new Card[] { Card, source }, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (decisions.Count > 0 && decisions.FirstOrDefault().Answer.Value)
                {
                    saidYes = (decisions.Count > 0 && decisions.FirstOrDefault().Answer.Value);
                }
            }

            if (saidYes && !dd.IsPretend)
            {
                IEnumerator coroutine = GameController.CancelActionEx(dd, isPreventEffect: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = DealDamage(CharacterCard, source, 1, DamageType.Melee, isCounterDamage: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.DestroyCard(DecisionMaker, Card, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                saidYes = false;
            }
        }
    }
}
