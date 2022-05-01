using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class FragmentStormCardController : CardController
	{
		private List<Card> actedstones;
		
		public FragmentStormCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			actedstones = new List<Card>();
		}

		public override IEnumerator Play()
		{
			int numberOfTargets = 1;
			int damageAmount = 2;
			int stoneDamageAmount = 2;
			List<DealDamageAction> storedDamage = new List<DealDamageAction>();
			GameController gameController = base.GameController;
			HeroTurnTakerController decisionMaker = DecisionMaker;
			DamageSource source = new DamageSource(base.GameController, base.CharacterCard);
			Func<Card, int?> amount = (Card c) => damageAmount;
			Func<int> dynamicNumberOfTargets = () => numberOfTargets;
			int? requiredTargets = numberOfTargets;
			CardSource cardSource = GetCardSource();
			IEnumerator coroutine = gameController.SelectTargetsAndDealDamage(decisionMaker, source, amount, DamageType.Melee, dynamicNumberOfTargets, optional: false, requiredTargets, isIrreducible: false, allowAutoDecide: false, null, null, storedDamage, null, null, selectTargetsEvenIfCannotDealDamage: true, null, null, includeReturnedTargets: true, ignoreBattleZone: false, cardSource);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			foreach (DealDamageAction item in storedDamage)
			{
				if (item.DidDestroyTarget)
				{
					break;
				}
				Card target = item.Target;
				int? targetPlayIndex = target.PlayIndex;
				SelectCardsDecision selectCardsDecision = new SelectCardsDecision(base.GameController, DecisionMaker, (Card c) => c.IsInPlay && c.DoKeywordsContain("stone", false, false), SelectionType.CardToDealDamage, null, isOptional: false, cardSource: GetCardSource(), requiredDecisions: null, eliminateOptions: true, allowAutoDecide: true, allAtOnce: false, dynamicNumberOfCards: () => DoesstoneDamageContinue(target));
				IEnumerator coroutine2 = base.GameController.SelectCardsAndDoAction(selectCardsDecision, (SelectCardDecision sc) => stoneDamageResponse(sc, target, stoneDamageAmount), null, () => target.PlayIndex != targetPlayIndex, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
				actedstones.Clear();
			}
		}
		private int DoesstoneDamageContinue(Card target)
		{
			if (!target.IsBeingDestroyed && target.Location.IsInPlay && !target.IsBeingDestroyed)
			{
				int num = FindCardsWhere((Card c) => c.DoKeywordsContain("stone", false, false) && c.IsInPlay).Except(actedstones).Count();
				return actedstones.Count() + num;
			}
			return 0;
		}

		private IEnumerator stoneDamageResponse(SelectCardDecision sc, Card target, int stoneDamageAmount)
		{
			Card selectedCard = sc.SelectedCard;
			actedstones.Add(selectedCard);
			IEnumerator coroutine = DealDamage(selectedCard, target, stoneDamageAmount, DamageType.Melee);
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