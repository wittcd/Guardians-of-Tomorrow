using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Hoplite
{

	public class HardlightHoplonCardController : CardController
	{

		public HardlightHoplonCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			AddReduceDamageTrigger((Card c) => c == base.CharacterCard, 1);
			AddTrigger((DealDamageAction dd) => CheckDamageCriteria(dd), (DealDamageAction dd) => GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), 3, DamageType.Melee, 1, false, 0, cardSource: GetCardSource()), TriggerType.DealDamage, TriggerTiming.After);
		}


		//checks if there's an attempt to deal damage to Hoplite which fails to deal damage (redirects don't count)
		private bool CheckDamageCriteria(DealDamageAction dd)
		{
			if (!dd.IsPretend && dd.Target == base.CharacterCard && !dd.DidDealDamage) {
				if (dd.OriginalAmount <= 0)
				{
					return dd.DamageModifiers.Where((ModifyDealDamageAction ga) => ga is IncreaseDamageAction).Any();
				}
				return true;
			}
			return false;
		}

	}
}