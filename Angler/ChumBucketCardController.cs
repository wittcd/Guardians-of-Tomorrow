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
    public class ChumBucketCardController : CardController
    {
        public ChumBucketCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public static readonly string FirstAnglerDestroyKey = "AnglerFirstTargetDestroyed";

        public override void AddTriggers()
        {
            //"The first time each turn {Angler} destroys a target, you may put a Bait card from your trash into your hand."
            AddTrigger<DestroyCardAction>((DestroyCardAction dc) => dc.CardSource != null && dc.CardSource.Card.Owner == TurnTaker && dc.CardToDestroy.Card.IsTarget && dc.WasCardDestroyed && (dc.DealDamageAction == null || dc.DealDamageAction.DamageSource.Card == base.CharacterCard) && (dc.DealDamageAction != null || dc.CardSource.Card.IsOneShot || dc.CardSource.Card.HasPowers || dc.ResponsibleCard == CharacterCard) && !GameController.GetCardPropertyJournalEntryBoolean(Card, FirstAnglerDestroyKey).Value, (DestroyCardAction dc) => GameController.SelectCardFromLocationAndMoveIt(DecisionMaker, TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("bait"), "bait"), new MoveCardDestination[] { new MoveCardDestination(TurnTaker.ToHero().Hand) }, cardSource: GetCardSource()), TriggerType.MoveCard, TriggerTiming.After);
            AddStartOfTurnTrigger((TurnTaker tt) => true, (PhaseChangeAction pc) => reset_target_destroyed(), TriggerType.Other);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{Angler} deals 1 target 1 melee damage. Play a Bait card."
            int powerNumeralTargets = GetPowerNumeral(0, 1);
            int powerNumeralDamage = GetPowerNumeral(1, 1);
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), powerNumeralDamage, DamageType.Melee, powerNumeralTargets, false, powerNumeralTargets, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectAndPlayCardFromHand(TurnTakerController.ToHero(), false, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("bait"), "bait"), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator GainBaitAndSetProperty()
        {
            GameController.AddCardPropertyJournalEntry(Card, FirstAnglerDestroyKey, true);
            IEnumerator coroutine = GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator reset_target_destroyed()
        {
            GameController.AddCardPropertyJournalEntry(Card, FirstAnglerDestroyKey, false);
            yield break;
        }
    }
}
