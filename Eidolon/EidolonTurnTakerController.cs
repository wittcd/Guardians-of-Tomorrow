using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
	public class EidolonTurnTakerController : TurnTakerController
	{
		public EidolonTurnTakerController(TurnTaker turnTaker, GameController gameController)
			: base(turnTaker, gameController)
		{
		}

	}
}
