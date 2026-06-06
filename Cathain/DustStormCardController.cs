using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace GuardiansOfTomorrow.Cathain
{
    public class DustStormCardController : CardController
    {
        public DustStormCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //When this card enters play, prevent the next damage dealt by each non-hero target and {Cathain} or one of your Loci deals itself 6 irreducible infernal damage.
            IEnumerator coroutine = GameController.SelectCardsAndDoAction(DecisionMaker, new LinqCardCriteria((Card c) => !IsHero(c) && c.IsTarget && c.IsInPlayAndHasGameText, "non-hero"), SelectionType.PreventDamage, PreventNextDamageDealt, allowAutoDecide: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

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
            // "At the start of each turn, {Cathain} deals up to 3 targets 1 projectile damage each.",
            AddStartOfTurnTrigger((TurnTaker tt) => true, (PhaseChangeAction pc) => GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Projectile, 3, false, 0, cardSource: GetCardSource()), TriggerType.DealDamage);
            //"At the end of your turn, if this card did not enter play this turn, destroy it."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DestroyThisCardResponse(null), TriggerType.DestroySelf, (PhaseChangeAction pc) => !DidEnterThisTurn(pc));
        }

        private IEnumerator PreventNextDamageDealt(Card c)
        {
            CannotDealDamageStatusEffect cddse = new CannotDealDamageStatusEffect();
            cddse.NumberOfUses = 1;
            cddse.SourceCriteria.IsSpecificCard = c;
            IEnumerator coroutine = GameController.AddStatusEffectEx(cddse, true, GetCardSource(), new Card[] { Card });
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
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
