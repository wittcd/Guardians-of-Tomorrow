using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class StalkedByGhostsCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public StalkedByGhostsCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		

		public override IEnumerator Play()
		{
			//Ninetails deals 1 target 3 infernal damage. Reveal cards until Ghost, then put it into play and shuffle others back.
			GameController gameController = base.GameController;
			HeroTurnTakerController decisionMaker = DecisionMaker;
			DamageSource source = new DamageSource(base.GameController, base.CharacterCard);
			int? numberOfTargets = 1;
			int? requiredTargets = 1;
			CardSource cardSource = GetCardSource();
			IEnumerator coroutine = gameController.SelectTargetsAndDealDamage(decisionMaker, source, 3, DamageType.Infernal, numberOfTargets, optional: false, requiredTargets, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, cardSource);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//reveal until ghost, play, reshuffle others
			coroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(base.TurnTakerController, base.TurnTaker.Deck, null, new LinqCardCriteria((Card c) => c.DoKeywordsContain("ghost", false, false), "ghost"), isPutIntoPlay: true, null, null, null, fromBottom: false, 1);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = base.GameController.ShuffleTrashIntoDeck(base.TurnTakerController, necessaryToPlayCard: false, null, GetCardSource());
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