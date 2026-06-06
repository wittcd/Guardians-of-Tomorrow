using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Cathain
{
    public class SinkingMorassCardController : CardController
    {
        public SinkingMorassCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //You may destroy up to 3 ongoing or environment cards.
            IEnumerator coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) || c.IsEnvironment, "ongoing or environment"), 3, false, 0, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{Cathain} or one of your Loci deals itself 6 irreducible infernal damage.
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 6, DamageType.Infernal, 1, false, 1, additionalCriteria: (Card c) => c == CharacterCard, cardSource: GetCardSource());
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
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsCard && !IsHeroTarget(dd.DamageSource.Card), (DealDamageAction dd) => 2);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DestroyThisCardResponse(null), TriggerType.DestroySelf, (PhaseChangeAction pc) => !DidEnterThisTurn(pc));
        }

        private bool DidEnterThisTurn(PhaseChangeAction pc)
        {
            List<CardEntersPlayJournalEntry> entries = Journal.CardEntersPlayEntriesThisTurn().ToList();
            foreach (CardEntersPlayJournalEntry entry in entries)
            {
                if (entry.Card == Card)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
