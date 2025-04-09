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
    public class DesignatedVictimCardController : CardController
    {
        public DesignatedVictimCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        //Play this next to a hero. Redirect damage dealt to other heroes to that hero. Redirect damage dealt to that hero to a Bait card. At the start of your turn, destroy this card.
        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            LinqCardCriteria validTargets = new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay && IsHeroCharacterCard(c), "hero");
            IEnumerator coroutine = SelectCardThisCardWillMoveNextTo(validTargets, storedResults, isPutIntoPlay, decisionSources);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DestroyThisCardResponse(null), TriggerType.DestroySelf);
            AddRedirectDamageTrigger((DealDamageAction dd) => IsHeroCharacterCard(dd.Target) && dd.Target != GetCardThisCardIsNextTo(), () => GetCardThisCardIsNextTo(), false);
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == GetCardThisCardIsNextTo(), (DealDamageAction dd) => GameController.SelectTargetAndRedirectDamage(DecisionMaker, (Card c) => c.DoKeywordsContain("bait"), dd, false, null, GetCardSource()), TriggerType.RedirectDamage, TriggerTiming.Before);
        }
    }
}
