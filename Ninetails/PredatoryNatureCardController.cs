using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class PredatoryNatureCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public PredatoryNatureCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
			AddInhibitorException((GameAction ga) => ga is PlayCardAction && base.Card.Location.IsHand);
		}

		private IEnumerator PlayFromHandResponse()
		{
			IEnumerator coroutine = base.GameController.SendMessageAction(base.Card.Title + " puts itself into play.", Priority.High, GetCardSource(), null, showCardSource: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			GameController gameController = base.GameController;
			TurnTakerController turnTakerController = base.TurnTakerController;
			Card card = base.Card;
			CardSource cardSource = GetCardSource();
			coroutine = gameController.PlayCard(turnTakerController, card, isPutIntoPlay: true, null, optional: false, null, null, evenIfAlreadyInPlay: false, null, null, null, associateCardSource: false, fromBottom: false, canBeCancelled: true, cardSource);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public override IEnumerator PerformEnteringGameResponse()
		{
			IEnumerator coroutine = ((!base.Card.IsInHand) ? base.PerformEnteringGameResponse() : PlayFromHandResponse());
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
			//cannot be redirected
			AddMakeDamageNotRedirectableTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard));
			//End of turn deal 1 other hero target 2 melee damage.
			//If no damage dealt, deal self 4 irreducible toxic damage
			AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => DealDamageToHeroAndMaybeSelfResponse(), TriggerType.DealDamage);
			
		}

		public override void AddStartOfGameTriggers()
		{
			AddTrigger((DrawCardAction d) => d.DrawnCard == base.Card, (DrawCardAction d) => PlayFromHandResponse(), new TriggerType[2]
			{
				TriggerType.PutIntoPlay,
				TriggerType.Hidden
			}, TriggerTiming.After, null, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: true);
			AddTrigger((MoveCardAction m) => m.Destination == base.HeroTurnTaker.Hand && m.CardToMove == base.Card, (MoveCardAction m) => PlayFromHandResponse(), new TriggerType[2]
			{
				TriggerType.PutIntoPlay,
				TriggerType.Hidden
			}, TriggerTiming.After, null, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: true);
		}

		public IEnumerator DealDamageToHeroAndMaybeSelfResponse() 
		{
			DamageSource source = new DamageSource(base.GameController, base.CharacterCard);
			Func<Card, bool> additionalCriteria = (Card c) => c.IsHero && c != base.CharacterCard;
			List<DealDamageAction> storedResultsDamage = new List<DealDamageAction>();
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, source, 2, DamageType.Melee, 1, optional: false, 1, isIrreducible: false, allowAutoDecide: false, autoDecide: false, additionalCriteria, null, storedResultsDamage, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (storedResultsDamage.FirstOrDefault() == null || !storedResultsDamage.FirstOrDefault().DidDealDamage) 
			{
				
				
				coroutine = DealDamage(base.CharacterCard, base.CharacterCard, 4, DamageType.Toxic, isIrreducible: true, optional: false, isCounterDamage: false, null, null);
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
			//destroy a target with 3 (power numeral) or fewer HP.
            //If a target is destroyed this way, regain 3 (power numeral) HP
			//and shuffle this card into your deck.
			int toDestroy = GetPowerNumeral(0, 3);
			int heal = GetPowerNumeral(1, 3);
			List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
			IEnumerator coroutine = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsTarget && c.HitPoints.Value <= toDestroy, "targets with " + toDestroy + " or fewer HP", useCardsSuffix: false), optional: false, storedResults, null, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (storedResults.Count() > 0)
			{
				IEnumerator coroutine3 = base.GameController.GainHP(base.CharacterCard, heal, null, null, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine3);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine3);
				}
				coroutine3 = base.GameController.ShuffleCardIntoLocation(DecisionMaker, base.Card, base.TurnTaker.Deck, optional: false, toBottom: false, GetCardSource());
					if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine3);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine3);
				}
			}
		}
	}
}