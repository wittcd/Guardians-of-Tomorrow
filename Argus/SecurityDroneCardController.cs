using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
namespace GuardiansOfTomorrow.Argus
{
    public class SecurityDroneCardController : CamdroneCardController
    {
        public SecurityDroneCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
			//deal 1 target 1 projectile and 1 lightning damage
			int powerNumeral = GetPowerNumeral(0, 1);
			int powerNumeral2 = GetPowerNumeral(1, 1);
			int powerNumeral3 = GetPowerNumeral(2, 1);
			List<DealDamageAction> list = new List<DealDamageAction>();
			list.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.CharacterCard), null, powerNumeral2, DamageType.Projectile));
			list.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.CharacterCard), null, powerNumeral3, DamageType.Lightning));
			IEnumerator coroutine = SelectTargetsAndDealMultipleInstancesOfDamage(list, null, null, powerNumeral, powerNumeral);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public override IEnumerator ActivateOperate() 
		{
			IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
			idse.SourceCriteria.IsSpecificCard = base.CharacterCard;
			idse.NumberOfUses = 1;
			IEnumerator coroutine = AddStatusEffect(idse, true);
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
