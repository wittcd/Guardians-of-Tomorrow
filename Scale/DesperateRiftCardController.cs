using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class DesperateRiftCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public DesperateRiftCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			LinqCardCriteria isBalance = new LinqCardCriteria((Card c) => c.DoKeywordsContain("balance"), "balance cards");
			//play balance
			IEnumerator coroutine = SelectAndPlayCardFromHand(DecisionMaker, false, null, isBalance, false, false, true, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//deal 3 targets 3 inf damage each
			GameController gameController = base.GameController;
			HeroTurnTakerController decisionMaker = DecisionMaker;
			DamageSource source = new DamageSource(base.GameController, base.CharacterCard);
			int? numberOfTargets = 3;
			int? requiredTargets = 3;
			CardSource cardSource = GetCardSource();
			coroutine = gameController.SelectTargetsAndDealDamage(decisionMaker, source, 3, DamageType.Infernal, numberOfTargets, optional: false, requiredTargets, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, cardSource);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//discard each top card
			Func<Location, bool> locationCriteria = (Location l) => !l.OwnerTurnTaker.IsIncapacitatedOrOutOfGame;
			coroutine = gameController.DiscardTopCardsOfDecks(decisionMaker, locationCriteria, 1, null, null, null, null, TurnTaker, cardSource);
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