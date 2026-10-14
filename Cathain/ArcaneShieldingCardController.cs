using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace GuardiansOfTomorrow.Cathain
{
    public class ArcaneShieldingCardController : CardController
    {
        public ArcaneShieldingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            //Redirect damage dealt to {Cathain} to one of your Loci.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == CharacterCard, (DealDamageAction dd) => GameController.SelectTargetAndRedirectDamage(DecisionMaker, (Card c) => c.Owner == TurnTaker && GameController.DoesCardContainKeyword(c, "locus"), dd, cardSource: GetCardSource()), TriggerType.RedirectDamage, TriggerTiming.Before);
            //At the start of turn, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DestroyThisCardResponse(null), TriggerType.DestroySelf);
        }

        private IEnumerator RedirectAndMakeIrreducible(DealDamageAction dd)
        {
            List<Card> availableLoci = FindCardsWhere((Card c) => GameController.DoesCardContainKeyword(c, "locus") && c.Owner == TurnTaker).ToList();
            if (availableLoci.Count != 0)
            {
                IEnumerator coroutine = GameController.SelectTargetAndRedirectDamage(DecisionMaker, (Card c) => availableLoci.Contains(c), dd, true, null, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.MakeDamageIrreducible(dd, GetCardSource());
            }
            else
            {
                IEnumerator coroutine = GameController.DoNothing();
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
