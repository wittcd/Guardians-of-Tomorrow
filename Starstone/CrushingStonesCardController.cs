using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class CrushingStonesCardController : CardController
	{
		public CrushingStonesCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.Location == base.TurnTaker.PlayArea && c.DoKeywordsContain("stone", false, false)), storedResults, optional: false, allowAutoDecide: false, null, includeRealCardsOnly: true, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			SelectCardDecision selectCardDecision = storedResults.FirstOrDefault();
			if (selectCardDecision != null && selectCardDecision.SelectedCard != null)
			{
				DamageSource ds = new DamageSource(base.GameController, selectCardDecision.SelectedCard);
				coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, ds, selectCardDecision.SelectedCard.HitPoints.Value, DamageType.Melee, 1, false, 1, false, false, false, null, null, null, null, null, false, null, null, false, null, GetCardSource());
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