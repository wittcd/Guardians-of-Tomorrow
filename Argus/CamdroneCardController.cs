using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace GuardiansOfTomorrow.Argus
{
    public abstract class CamdroneCardController : CardController
	{
		public CamdroneCardController(Card card, TurnTakerController turnTakerController)
		   : base(card, turnTakerController)
		{
		}
		public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition definition)
		{
			IEnumerator enumerator = null;
			if (definition.Name == "operate")
			{
				enumerator = ActivateOperate();
			}
			if (enumerator != null)
			{
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(enumerator);
				}
				else
				{
					base.GameController.ExhaustCoroutine(enumerator);
				}
			}
		}

		public abstract IEnumerator ActivateOperate();
	}
}
