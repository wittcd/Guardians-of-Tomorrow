using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class DancingFlameCardController : CardController
	{

		public DancingFlameCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}


		public override IEnumerator Play()
		{
			List<DealDamageAction> list = new List<DealDamageAction>();
			list.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.CharacterCard), null, 1, DamageType.Fire));
			list.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.CharacterCard), null, 1, DamageType.Melee));
			List<DealDamageAction> sra = new List<DealDamageAction>();
			IEnumerator coroutine = SelectTargetsAndDealMultipleInstancesOfDamage(list, null, null, 1, 1, false, sra);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			DealDamageAction dd = sra.FirstOrDefault();
			if (dd != null && dd.OriginalTarget != null) 
			{
				List<DiscardCardAction> srd = new List<DiscardCardAction>();
				coroutine = SelectAndDiscardCards(DecisionMaker, 1, optional: true, null, srd);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

				if (DidDiscardCards(srd)) 
				{
					RedirectDamageStatusEffect redirectDamageStatusEffect = new RedirectDamageStatusEffect();
					redirectDamageStatusEffect.TargetCriteria.IsSpecificCard = base.CharacterCard;
					redirectDamageStatusEffect.RedirectableTargets.IsSpecificCard = dd.OriginalTarget;
					redirectDamageStatusEffect.NumberOfUses = 1;
					redirectDamageStatusEffect.IsOptional = false;
					coroutine = AddStatusEffect(redirectDamageStatusEffect);
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