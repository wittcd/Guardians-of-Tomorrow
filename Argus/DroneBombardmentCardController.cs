using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Argus
{

	public class DroneBombardmentCardController : CardController
	{
		public DroneBombardmentCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}


		public override IEnumerator Play()
		{
			//1 target 1 damage x times where x is number of camdrones.
			LinqCardCriteria isCamdrone = new LinqCardCriteria((Card c) => c.DoKeywordsContain("camdrone") && c.IsInPlay, "Camdrone in play");
			List<DealDamageAction> list = new List<DealDamageAction>();
			int amount = FindCardsWhere(isCamdrone, null, false).Count();
			for (int i = 0; i < amount; i++)
			{
				list.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, base.CharacterCard), null, 1, DamageType.Projectile, false, null, null, null, false));
			}
			if (list.Count() > 0)
			{
				IEnumerator coroutine = SelectTargetAndDealMultipleInstancesOfDamage(list, null, null);
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
