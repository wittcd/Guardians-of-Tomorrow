using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using GuardiansOfTomorrow.TheReverseEngineer;

namespace GuardiansOfTomorrow.TheReverseEngineer
{
    abstract class TheReverseEngineerCardController : CardController
    {
        public TheReverseEngineerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            if (TheReverseEngineerExtensions.WasCardPlayedFromTrash(this, Journal, Card))
            {
                IEnumerator coroutine = FromTrashAction();
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

        public abstract IEnumerator FromTrashAction();
    }
}
