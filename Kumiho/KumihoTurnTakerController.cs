using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Kumiho
{
    public class KumihoTurnTakerController : TurnTakerController
    {
		public KumihoTurnTakerController(TurnTaker turnTaker, GameController gameController)
			: base(turnTaker, gameController)
		{
		}

		public override IEnumerator StartGame()
		{
			IEnumerator coroutine = PutCardIntoPlay("StolenSoul", true);
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
