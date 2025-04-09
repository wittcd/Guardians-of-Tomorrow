using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class TwistingCinderCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public TwistingCinderCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//end of turn fire damage stuff
			AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, EndOfTurnResponse, TriggerType.DealDamage);
			//when destroyed, draw 2
			AddWhenDestroyedTrigger(OnDestroyResponse, TriggerType.DrawCard);
		}

		private IEnumerator OnDestroyResponse(DestroyCardAction dc)
		{
			IEnumerator coroutine = DrawCard(base.HeroTurnTaker);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = DrawCard(base.HeroTurnTaker);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public IEnumerator EndOfTurnResponse(PhaseChangeAction p)
		{
			List<SelectNumberDecision> storedNumber = new List<SelectNumberDecision>();
			IEnumerator coroutine = base.GameController.SelectNumber(DecisionMaker, SelectionType.DealDamage, 0, 3, optional: false, allowAutoDecide: false, null, storedNumber, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			SelectNumberDecision selectNumberDecision = storedNumber.FirstOrDefault();
			if (selectNumberDecision != null && selectNumberDecision.SelectedNumber.HasValue)
			{
				DamageSource source = new DamageSource(base.GameController, base.Card);
				CardSource cardSource = GetCardSource();
				List<DealDamageAction> storedDamage = new List<DealDamageAction>();
				coroutine = base.GameController.SelectTargetsAndDealDamage(base.DecisionMaker, source, selectNumberDecision.SelectedNumber.Value, DamageType.Fire, 1, optional: false, 0, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null, null, storedDamage, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, cardSource);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

				DealDamageAction dd = storedDamage.FirstOrDefault();
				if (dd != null && dd.Amount >= 3) 
				{ 
					coroutine = base.GameController.DestroyCard(DecisionMaker, base.Card, optional: false, null, null, null, null, null, null, null, null, cardSource);
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