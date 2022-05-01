using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Scale
{

	public class FoldSpaceCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public FoldSpaceCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//reduce next damage dealt to scale by 2
			ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(GetPowerNumeral(0, 2));
			reduceDamageStatusEffect.TargetCriteria.IsSpecificCard = base.CharacterCard;
			reduceDamageStatusEffect.NumberOfUses = 1;
			reduceDamageStatusEffect.CardDestroyedExpiryCriteria.Card = base.Card;
			IEnumerator coroutine = AddStatusEffect(reduceDamageStatusEffect);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//deal 1 target 1 melee and 1 energy
			List<DealDamageAction> list = new List<DealDamageAction>();
			list.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.CharacterCard), null, 1, DamageType.Melee));
			list.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.CharacterCard), null, 1, DamageType.Energy));
			coroutine = SelectTargetsAndDealMultipleInstancesOfDamage(list, null, null, 1, 1);
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