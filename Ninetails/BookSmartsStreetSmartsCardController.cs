using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class BookSmartsStreetSmartsCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public BookSmartsStreetSmartsCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}



		public override IEnumerator Play()
		{
			//either draw 3 or redirect next damage dealt to ninetails to someone else
			List<Function> list = new List<Function>();
			Function item = new Function(DecisionMaker, "Draw 3 cards", SelectionType.DrawCard, () => DrawCards(DecisionMaker, 3), null, null, "Draw 3 cards");
			list.Add(item);
			RedirectDamageStatusEffect rdse = new RedirectDamageStatusEffect();
			rdse.TargetCriteria.IsSpecificCard = base.CharacterCard;
			rdse.RedirectableTargets.IsTarget = true;
			rdse.NumberOfUses = 1;
			rdse.IsOptional = false;
			Function item2 = new Function(DecisionMaker, "Redirect Next Damage", SelectionType.RedirectNextDamage, () => AddStatusEffect(rdse), null, null, "Redirect the next damage dealt to Ninetails to another target");
			list.Add(item2);
			SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, DecisionMaker, list, optional: false, null, null, null, GetCardSource());
			IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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