using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Deluge
{
    class DelugeCharacterCardController : VillainCharacterCardController
    {
		public DelugeCharacterCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			SpecialStringMaker.ShowNumberOfCardsUnderCard(card);
        }

        public override void AddTriggers()
        {
            AddDefeatedIfDestroyedTriggers(false);
            /*if (IsGameChallenge)
            {
                AddTrigger((DestroyCardAction dc) => dc.CardToDestroy.Card.DoKeywordsContain("flood") && dc.CardSource != null && dc.CardSource.Card.Owner == TurnTaker && dc.CardToDestroy.Card.IsTarget && dc.WasCardDestroyed && (dc.DealDamageAction == null || !dc.DealDamageAction.DamageSource.Card.IsEnvironment) && (dc.DealDamageAction != null || dc.CardSource.Card.IsOneShot || dc.CardSource.Card.HasPowers || !dc.ResponsibleCard.IsEnvironment), (DestroyCardAction dc) => PlayTheTopCardOfTheVillainDeckResponse(dc), TriggerType.PlayCard, TriggerTiming.After);
            }*/
            base.AddTriggers();
        }
        public override void AddSideTriggers()
        {
            if (!CharacterCard.IsFlipped)
            {
                if (!IsGameChallenge)
                {
                    AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => PlayTheTopCardOfTheVillainDeckResponse(pc), TriggerType.PlayCard));
                }
                if (IsGameAdvanced) {
                    AddSideTrigger(AddReduceDamageTrigger((Card c) => IsVillain(c), 1));
                }
                base.Card.UnderLocation.OverrideIsInPlay = false;
                AddSideTrigger(AddTrigger((DestroyCardAction dc) => dc.CardToDestroy.Card.IsEnvironment || dc.CardToDestroy.Card.DoKeywordsContain("flood"), (DestroyCardAction dc) => RespondToCardDestructionIfCorrectType(dc), TriggerType.MoveCard, TriggerTiming.After));
                //AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == CharacterCard.Owner && FindCardsWhere((Card c) => c.Location == Card.UnderLocation).Count() <= 0, FlipThisCharacterCardResponse, TriggerType.FlipCard));
                AddSideTrigger(AddTrigger((MoveCardAction mc) => mc.Destination == CharacterCard.UnderLocation && CharacterCard.UnderLocation.NumberOfCards >= 10, (MoveCardAction mc) => GameController.GameOver(EndingResult.AlternateDefeat, "Everything is flooded. The heroes lose the game!", showEndingTextAsMessage: true, cardSource: GetCardSource()), TriggerType.GameOver, TriggerTiming.After));
            }
            else
            {
                if (IsGameAdvanced)
                {
                    AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => !IsVillain(dd.Target), 1));
                }
                if (IsGameChallenge)
                {
                    AddTrigger((DestroyCardAction dc) => dc.CardToDestroy.Card.DoKeywordsContain("flood") || IsOngoing(dc.CardToDestroy.Card), PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard, TriggerTiming.After);
                }
                AddSideTrigger(AddTrigger((CardEntersPlayAction cep) => (cep.CardEnteringPlay.DoKeywordsContain("flood") && !cep.CardEnteringPlay.IsMinion) || (cep.CardEnteringPlay.IsVillain && IsOngoing(cep.CardEnteringPlay)), (CardEntersPlayAction cep) => FlippedPlayFloodResponse(cep), TriggerType.DestroyCard, TriggerTiming.After));
                AddSideTrigger(AddTrigger((DestroyCardAction dc) => dc.CardToDestroy.Card.IsEnvironment, (DestroyCardAction dc) => PlayTheTopCardOfTheVillainDeckResponse(dc), TriggerType.PlayCard, TriggerTiming.After));
                AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => !IsVillain(c), 2, DamageType.Projectile, cardSource: GetCardSource()), TriggerType.DealDamage));
            }
            base.AddSideTriggers();
        }

        private IEnumerator FlippedPlayFloodResponse(CardEntersPlayAction cep)
        {
            /*IEnumerator coroutine = GameController.SendMessageAction("A flood entered play, destroying...", Priority.Critical, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }*/

            IEnumerator coroutine = base.GameController.DestroyCard(DecisionMaker, cep.CardEnteringPlay, optional: false, null, null, null, null, null, null, null, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c1) => IsHero(c1), (Card c2) => H, DamageType.Lightning);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator RespondToCardDestructionIfCorrectType(DestroyCardAction dc)
        {
            if (dc.CardToDestroy.Card.IsEnvironment && dc.CardSource != null && dc.WasCardDestroyed && (dc.DealDamageAction == null || dc.DealDamageAction.DamageSource.Card != dc.CardToDestroy.Card) && (dc.DealDamageAction != null || dc.CardSource.Card.IsOneShot || dc.CardSource.Card.HasPowers || dc.ResponsibleCard != dc.CardToDestroy.Card))
            {
                dc.SetPostDestroyDestination(CharacterCard.UnderLocation, cardSource: GetCardSource());
            }
            else if (dc.CardToDestroy.Card.DoKeywordsContain("flood"))
            {
                IEnumerator coroutine = GameController.MoveCard(DecisionMaker, CharacterCard.UnderLocation.TopCard, CharacterCard.UnderLocation.TopCard.Owner.Trash, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (CharacterCard.UnderLocation.NumberOfCards == 0)
                {
                    coroutine = FlipThisCharacterCardResponse(dc);
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
            /*if (CharacterCard.UnderLocation.NumberOfCards >= 10)
            {
                    IEnumerator coroutine = GameController.GameOver(EndingResult.AlternateDefeat, "Everything is flooded. The heroes lose the game!", showEndingTextAsMessage: true, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
            }*/
            IEnumerator coroutine2 = GameController.DoNothing();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine = base.AfterFlipCardImmediateResponse();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = base.GameController.ChangeMaximumHP(base.Card, base.Card.Definition.FlippedHitPoints.Value, alsoSetHP: true, GetCardSource());
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
