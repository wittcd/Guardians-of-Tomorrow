using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace GuardiansOfTomorrow.Arsenal
{
    abstract class ArsenalRelicCardController : CardController
    {
        public ArsenalRelicCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public IEnumerator ReturnAllButTwoRelicsToHand()
        {
            List<Card> relics = FindCardsWhere((Card c) => c.IsRelic && c.Owner == TurnTaker && c.Location == TurnTaker.PlayArea && c.IsInPlayAndNotUnderCard).ToList();
            int numToMove = relics.Count - 2;
            if (numToMove > 0)
            {
                MoveCardDestination m = new MoveCardDestination(TurnTaker.ToHero().Hand);
                List<MoveCardDestination> mcds = new List<MoveCardDestination>();
                mcds.Add(m);
                IEnumerator coroutine = GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, TurnTaker.PlayArea, numToMove, numToMove, new LinqCardCriteria((Card c) => relics.Contains(c), "relics in your play area"), mcds, selectionType: SelectionType.MoveCardToHand, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                IEnumerator coroutine = DoNothing();
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

        public IEnumerator MoveCardToHand(Card c)
        {
            IEnumerator coroutine = GameController.MoveCard(TurnTakerController, c, TurnTaker.ToHero().Hand, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public int GetNumberOfMeleeDealtThisTurn()
        {
            int count = 0;
            foreach (DealDamageJournalEntry entry in Journal.DealDamageEntriesThisTurn())
            {
                if (entry.DamageType == DamageType.Melee && entry.SourceCard == CharacterCard)
                {
                    count++;
                }
            }

            return count;
        }

        public int GetNumberOfProjectileDealtThisTurn()
        {
            int count = 0;
            foreach (DealDamageJournalEntry entry in Journal.DealDamageEntriesThisTurn())
            {
                if (entry.DamageType == DamageType.Projectile && entry.SourceCard == CharacterCard)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
