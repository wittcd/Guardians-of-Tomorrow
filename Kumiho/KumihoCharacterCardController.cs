using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Kumiho
{
    class KumihoCharacterCardController : VillainCharacterCardController
    {
		public KumihoCharacterCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
			base.SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => !base.Card.IsFlipped;
			base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("ghost"), "ghost"), () => true).Condition = () => base.Card.IsFlipped;
		}
		/*
		  "{Kumiho} is indestructible",
          "{Kumiho} is immune to damage dealt by the hero next to Stolen Soul.",
          "The first time Kumiho is dealt damage by a hero target each turn, redirect that damage to the hero target with the highest HP.",
          "If the hero next to Stolen Soul is incapacitated, their soul is devoured and the heroes lose the game.",
          "At the start of the villain turn, if Stolen Soul is not in play, {Kumiho} flips."
		*/

		//"advanced": "At the end of the villain turn, play the top card of the villain deck.",
		//"flippedAdvanced": "Increase damage dealt by Kumiho and Ghosts by 1.",

		//"challengeText": "N/A (?)",

		public override void AddTriggers()
		{
			AddDefeatedIfDestroyedTriggers(false);
			base.AddTriggers();
		}

        public override void AddSideTriggers()
        {
			if (!base.Card.IsFlipped)
            {
				if(base.IsGameAdvanced)
                {
					AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.PlayTheTopCardOfTheVillainDeckWithMessageResponse, TriggerType.PlayCard));
				}

				AddSideTrigger(AddTrigger((DealDamageAction dd) => dd.DamageSource.Card.IsHeroCharacterCard, SoulIncreaseDamageResponse, TriggerType.IncreaseDamage, TriggerTiming.Before));
				AddSideTrigger(AddTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && dd.DamageSource.Card.IsHeroCharacterCard, SoulHighestHPRedirectResponse, TriggerType.RedirectDamage, TriggerTiming.Before));
				AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pc) => FlipThisCharacterCardResponse(null), TriggerType.FlipCard, (PhaseChangeAction pc) => FindCardsWhere((Card c) => c.IsInPlay && c.Identifier == "StolenSoul").Count() == 0 ));
            }
			else
            {
				AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == base.Card || dd.DamageSource.Card.DoKeywordsContain("ghost"), H - 2));
				if (base.IsGameAdvanced)
                {
					AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.CardSource.Card == base.Card || dd.CardSource.Card.DoKeywordsContain("ghost"), 1));
				}
				AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageAndMaybePlayTopCardResponse, TriggerType.DealDamage));
            }

			base.AddSideTriggers();
        }

		public override bool AskIfCardIsIndestructible(Card card)
		{
			if (card == base.Card && !base.Card.IsFlipped)
			{
				return card.BattleZone == base.CardWithoutReplacements.BattleZone;
			}
			return false;
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
			if (base.Card.HitPoints < 25)
            {
				coroutine = GameController.SetHP(base.Card, 25, GetCardSource());
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

		private IEnumerator DealDamageAndMaybePlayTopCardResponse(PhaseChangeAction pc)
        {
			IEnumerator coroutine = DealDamage((Card c) => c == base.Card, (Card c) => c.IsHero, (Card c) => 1, DamageType.Infernal);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if (FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("ghost")).Count() <= 0)
            {
				coroutine = base.PlayTheTopCardOfTheVillainDeckResponse(null);
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
		private IEnumerator SoulIncreaseDamageResponse(DealDamageAction dd)
        {
			List<Card> findSoul = FindCardsWhere((Card c) => c.Identifier == "StolenSoul" && c.IsInPlayAndHasGameText).ToList();
			if (findSoul.Count() > 0)
			{
				Card soul = findSoul.FirstOrDefault();
				if (dd.DamageSource.Card == soul.Location.OwnerTurnTaker.CharacterCard)
				{
					IEnumerator coroutine = GameController.IncreaseDamage(dd, 1, false, GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
				}
				else
                {
					IEnumerator coroutine = DoNothing();
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
		private IEnumerator SoulHighestHPRedirectResponse(DealDamageAction dd)
		{
			List<Card> findSoul = FindCardsWhere((Card c) => c.Identifier == "StolenSoul" && c.IsInPlayAndHasGameText).ToList();
			if (findSoul.Count() > 0)
			{
				List<Card> stored = new List<Card>();
				Card soul = findSoul.FirstOrDefault();
				if (dd.DamageSource.Card == soul.Location.OwnerTurnTaker.CharacterCard)
				{
					IEnumerator coroutine = GameController.FindTargetsWithHighestHitPoints(1, 1, (Card c) => c.IsHeroCharacterCard && c != soul.Location.OwnerTurnTaker.CharacterCard, stored, cardSource: GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
					coroutine = RedirectDamage(dd, TargetType.HighestHP, (Card c) => stored.Contains(c));
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
				}
				else
                {
					IEnumerator coroutine = DoNothing();
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
}
