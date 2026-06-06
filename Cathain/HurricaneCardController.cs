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
    public class HurricaneCardController : CardController
    {
        public HurricaneCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //Play up to 2 cards from the top of the environment deck. {Cathain} deals up to 4 targets cold damage equal to the number of environment cards in play.
            IEnumerator coroutine = GameController.PlayTopCard(DecisionMaker, FindEnvironment(), false, 2, true, cardSource: GetCardSource());
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
            AddStartOfTurnTrigger((TurnTaker tt) => true, DestroyEnvAndDealDamage, TriggerType.DestroyCard);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DestroyThisCardResponse(null), TriggerType.DestroySelf, (PhaseChangeAction pc) => !DidEnterThisTurn(pc));
        }

        private IEnumerator DestroyEnvAndDealDamage(PhaseChangeAction pc)
        {
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), false, storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDestroyCard(storedResults))
            {
                List<DealDamageAction> damages = new List<DealDamageAction>();
                damages.Add(new DealDamageAction(GameController, new DamageSource(GameController, CharacterCard), null, 2, DamageType.Lightning));
                damages.Add(new DealDamageAction(GameController, new DamageSource(GameController, CharacterCard), null, 2, DamageType.Cold));
                
                coroutine = SelectTargetAndDealMultipleInstancesOfDamageEx(damages, cardSource: GetCardSource());
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
