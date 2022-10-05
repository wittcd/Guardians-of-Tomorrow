using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class UnholyEssenceCardController : AlterationCardController
    {
        public UnholyEssenceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = AlterationPlayedResponse(DamageType.Infernal, GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.ToHero().IsIncapacitatedOrOutOfGame, "active heroes"), SelectionType.ShuffleTrashIntoDeck, (TurnTaker tt) => base.GameController.ShuffleTrashIntoDeck(base.GameController.FindTurnTakerController(tt), necessaryToPlayCard: false, null, GetCardSource()), null, optional: false, null, null, allowAutoDecide: true, null, null, null, ignoreBattleZone: false, null, GetCardSource()));
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
