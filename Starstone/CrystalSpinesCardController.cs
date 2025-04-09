using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Starstone
{

	public class CrystalSpinesCardController : CardController
	{
		public CrystalSpinesCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowHasBeenUsedThisTurn("FirstDamageToStoneOrStarstoneThisTurn", "{0} has already taken effect this turn.", "{0} has not yet taken effect this turn.");
		}

		private const string FirstDamageToDeviceThisTurn = "FirstDamageToStoneOrStarstoneThisTurn";



		public override void AddTriggers()
		{
			AddTrigger((DealDamageAction dd) => !IsPropertyTrue("FirstDamageToStoneOrStarstoneThisTurn") && dd.DamageSource.IsTarget && !dd.DamageSource.IsHero && dd.DidDealDamage && (dd.Target.DoKeywordsContain("stone", evenIfUnderCard: true) || dd.Target.Identifier == "StarstoneCharacter"), FirstDamageDealtResponse, TriggerType.DealDamage, TriggerTiming.After, ActionDescription.DamageTaken);
			AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay("FirstDamageToStoneOrStarstoneThisTurn"), TriggerType.Hidden);
		}

		private IEnumerator FirstDamageDealtResponse(DealDamageAction dd)
		{
			SetCardPropertyToTrueIfRealAction("FirstDamageToStoneOrStarstoneThisTurn");
			IEnumerator coroutine = DealDamage(base.Card, dd.DamageSource.Card, 2, DamageType.Melee);
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