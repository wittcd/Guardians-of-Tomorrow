using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Hoplite
{
    class ImpenetrableRefugeCardController : CardController
    {
        public ImpenetrableRefugeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddTriggers()
        {
            AddPreventDamageTrigger((DealDamageAction dd) => true);
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => base.GameController.MoveCard(base.TurnTakerController, base.Card, base.TurnTaker.OutOfGame, toBottom: false, isPutIntoPlay: false, playCardIfMovingToPlayArea: true, null, showMessage: false, null, null, null, evenIfIndestructible: false, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, GetCardSource()), TriggerType.DestroySelf);
            AddAfterLeavesPlayAction((GameAction g) => base.GameController.DestroyAnyCardsThatShouldBeDestroyed(ignoreBattleZone: false, GetCardSource()), TriggerType.DestroyCard);
            AddTrigger((BulkMoveCardsAction bmc) => bmc.CardsToMove.Contains(base.Card) && bmc.Destination.IsOutOfGame, (BulkMoveCardsAction bmc) => base.GameController.DestroyAnyCardsThatShouldBeDestroyed(ignoreBattleZone: true, GetCardSource()), TriggerType.DestroyCard, TriggerTiming.After, ActionDescription.Unspecified, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: true);
            AddTrigger((SwitchBattleZoneAction sb) => sb.Origin == base.Card.BattleZone, (SwitchBattleZoneAction sb) => base.GameController.DestroyAnyCardsThatShouldBeDestroyed(ignoreBattleZone: true, GetCardSource()), TriggerType.DestroyCard, TriggerTiming.After, ActionDescription.Unspecified, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: false, null, null, ignoreBattleZone: true);
            AddTrigger((MoveCardAction mc) => mc.Origin.BattleZone == base.BattleZone && mc.Destination.BattleZone != base.BattleZone, (MoveCardAction mc) => base.GameController.DestroyAnyCardsThatShouldBeDestroyed(ignoreBattleZone: true, GetCardSource()), TriggerType.DestroyCard, TriggerTiming.After, ActionDescription.Unspecified, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: false, null, null, ignoreBattleZone: true);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            if (card != base.Card)
            {
                return card.BattleZone == base.CardWithoutReplacements.BattleZone;
            }
            return false;
        }
    }
}
