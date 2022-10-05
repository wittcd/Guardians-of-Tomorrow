using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheHatter
{
    class TheHatterCharacterCardController : VillainCharacterCardController
    {
		public TheHatterCharacterCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("thrall"), "thrall"), () => true);
			base.SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => !base.Card.IsFlipped;
			base.SpecialStringMaker.ShowHeroTargetWithLowestHP(1, 2, false).Condition = () => base.Card.IsFlipped;
		}
		/*
		* "Whenever a Thrall is destroyed, each hero discards a card.",
        * "At the end of the villain turn, shuffle the villain trash, then reveal cards from it until a Thrall is revealed. Put it into play and put the other revealed cards back in the trash. Then, the Hatter deals the hero target with the highest HP {H-1} projectile damage.",
        * "At the start of the villain turn, if there are 1 or fewer Thralls in play, the Hatter flips."
		*/

		//"advanced": "Increase damage dealt to hero targets by 1.",
		//"flippedAdvanced": "At the end of the villain turn, play the top card of the villain deck.",

		//"challengeText": "The first time each turn {The Hatter} would be dealt damage, redirect that damage to a Thrall.",

		public override void AddTriggers()
		{
			//base.AddTriggers();
			if (base.IsGameChallenge)
			{
				//"challengeText": "The first time each turn {The Hatter} would be dealt damage, redirect that damage to a Thrall",
				AddFirstTimePerTurnRedirectTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard, "dealt damage", TargetType.SelectTarget, (Card c) => c.DoKeywordsContain("thrall"));
			}
			AddTrigger((DestroyCardAction dc) => dc.CardToDestroy.Card.DoKeywordsContain("thrall"), AllPlayersMustDiscardResponse, TriggerType.DiscardCard, TriggerTiming.After);
			AddDefeatedIfDestroyedTriggers(false);
			base.AddTriggers();
		}

        public override void AddSideTriggers()
        {
			if (!base.Card.IsFlipped)
            {
				if(base.IsGameAdvanced)
                {
					AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.Target.IsHero, 1));
                }
				//AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, BringThrallBackResponse, TriggerType.PutIntoPlay));
				AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => c.IsHero, (Card c) => H - 1, DamageType.Projectile), TriggerType.DealDamage));
				AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.FlipThisCharacterCardResponse, TriggerType.FlipCard, (PhaseChangeAction p) => FindCardsWhere((Card c) => c.DoKeywordsContain("thrall") && c.IsInPlayAndHasGameText).Count() <= 1));
            }
			else
            {
                if (base.IsGameAdvanced)
                {
					AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.PlayTheTopCardOfTheVillainDeckWithMessageResponse, TriggerType.PlayCard));
				}
				//AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => c.IsHeroCharacterCard, (Card c) => base.H - 2, DamageType.Psychic, false, false, null, 2, null, null, false), TriggerType.DealDamage));
				AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DamageThenDestroyResponse, TriggerType.DealDamage));
				AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.FlipThisCharacterCardResponse, TriggerType.FlipCard, (PhaseChangeAction p) => FindCardsWhere((Card c) => c.DoKeywordsContain("thrall") && c.IsInPlayAndHasGameText).Count() >= 2));
            }

			base.AddSideTriggers();
        }

        private IEnumerator AllPlayersMustDiscardResponse(DestroyCardAction dc)
        {
			IEnumerator coroutine = base.GameController.EachPlayerDiscardsCards(1, 1, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
            {
				yield return base.GameController.StartCoroutine(coroutine);
            }
			else
            {
				base.GameController.ExhaustCoroutine(coroutine);
            }
        }

		private IEnumerator BringThrallBackResponse(PhaseChangeAction phaseChange)
		{
			bool tryPlaying = true;
			string message = base.Card.Title + " plays a Thrall card from the Trash.";
			if (!base.TurnTaker.Trash.Cards.Any((Card c) => c.DoKeywordsContain("thrall")));
			{
				tryPlaying = false;
				message = "There are no Thrall cards in The Hatter's trash to play.";
			}
			IEnumerator coroutine = base.GameController.SendMessageAction(message, Priority.Medium, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (tryPlaying)
			{
				IEnumerator coroutine2 = ReviveCardFromTrash(base.TurnTakerController, (Card c) => c.DoKeywordsContain("thrall"));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}

			/*coroutine = DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => c.IsHero, (Card c) => H - 1, DamageType.Projectile);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}*/
		}

		private IEnumerator DamageThenDestroyResponse(PhaseChangeAction pc)
        {
			IEnumerator coroutine = base.DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => c.IsHeroCharacterCard, (Card c) => H - 2, DamageType.Psychic, false, false, null, 2, null, DestroyAfterDamageResponse, false );
				if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		private IEnumerator DestroyAfterDamageResponse(DealDamageAction dd)
        {
			Card target = dd.Target;
			LinqCardCriteria isOwnedByTarget = new LinqCardCriteria((Card c) => c.Owner.CharacterCard == target && (c.IsOngoing || IsEquipment(c)), "ongoing or equipment card");
			if (dd.DidDealDamage)
			{
				IEnumerator coroutine = base.GameController.SelectAndDestroyCard(DecisionMaker, isOwnedByTarget, false, cardSource: GetCardSource());
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
