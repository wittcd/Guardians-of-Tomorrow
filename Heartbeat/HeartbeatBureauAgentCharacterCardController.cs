using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Heartbeat
{
    class HeartbeatBureauAgentCharacterCardController: HeroCharacterCardController
    {
        public static readonly string TargetedCardKey = "HeartbeatProtectedCard";
        public static readonly string HasShielded = "HeartbeatHasShielded";
        public HeartbeatBureauAgentCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.Owner == TurnTaker && IsOngoing(c), "Ongoing"));
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => ResetJournalFlags(), TriggerType.Other);
            AddTrigger((DealDamageAction dd) => dd.Target == GetCardPropertyJournalEntryCard(TargetedCardKey) && GetCardPropertyJournalEntryBoolean(HasShielded).Value, DestroyOngoingToPrevent, TriggerType.CancelAction, TriggerTiming.Before);
        }

        private IEnumerator ResetJournalFlags()
        {
            GameController.AddCardPropertyJournalEntry(Card, HasShielded, false);
            GameController.AddCardPropertyJournalEntry(Card, TargetedCardKey, (Card)null);
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

        private IEnumerator DestroyOngoingToPrevent(DealDamageAction dd)
        {
            List<DestroyCardAction> destroys = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.Owner == TurnTaker && IsOngoing(c), "your ongoing"), true, destroys, Card, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDestroyCard(destroys))
            {
                coroutine = GameController.CancelActionEx(dd, false, cardSource: GetCardSource());
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

        public override IEnumerator UsePower(int index = 0)
        {
            //Select a hero. The next time that hero is dealt damage before the start of your turn, you may destroy an Arcane Charge. If you do, prevent that damage.
            List<SelectTargetDecision> selects = new List<SelectTargetDecision>();
            IEnumerator coroutine = GameController.SelectTargetAndStoreResults(DecisionMaker, FindCardsWhere((Card c) => c.IsHeroCharacterCard), selects, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (selects.FirstOrDefault() != null && selects.FirstOrDefault().SelectedCard != null)
            {
                GameController.AddCardPropertyJournalEntry(Card, TargetedCardKey, selects.FirstOrDefault().SelectedCard);
                GameController.AddCardPropertyJournalEntry(Card, HasShielded, true);
            }
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        IEnumerator coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        yield break;
                    }
                case 1:
                    {
                        IEnumerator coroutine = GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        yield break;
                    }
                case 2:
                    {
                        ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(2);
                        rdse.TargetCriteria.IsHero = true;
                        rdse.NumberOfUses = 1;
                        IEnumerator coroutine = GameController.AddStatusEffectEx(rdse, true, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        yield break;
                    }
            }
        }
    }
}
