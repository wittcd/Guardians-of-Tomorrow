using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Ludramor
{
    class PlanetcrackerBargeCardController : CardController
    {
        public PlanetcrackerBargeCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(() => FindEnvironment().TurnTaker.Trash);
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //This card is indestructible so long as it has > 0 HP
            if (card == base.Card && card.HitPoints > 0)
            {
                return card.BattleZone == base.CardWithoutReplacements.BattleZone;
            }
            return false;
        }

        public override void AddTriggers()
        {
            TriggerType[] types = {TriggerType.DealDamage, TriggerType.RemoveFromGame};
            AddEndOfTurnTrigger((TurnTaker tt) => tt.IsEnvironment, (PhaseChangeAction pc) => GameController.DealDamage(DecisionMaker, Card, (Card c) => !IsVillain(c), FindEnvironment().TurnTaker.Trash.NumberOfCards, DamageType.Energy, cardSource: GetCardSource()), TriggerType.DealDamage);
            AddWhenDestroyedTrigger((DestroyCardAction dc) => DestructionResponse(dc), types);
        }

        private IEnumerator DestructionResponse(DestroyCardAction dc)
        {
            IEnumerator coroutine = GameController.DealDamage(DecisionMaker, Card, (Card c) => IsVillain(c) && c != Card, 5, DamageType.Psychic, true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.MoveCard(TurnTakerController, Card, TurnTaker.OutOfGame, cardSource: GetCardSource());
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
