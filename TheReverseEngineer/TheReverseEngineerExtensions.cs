using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.TheReverseEngineer
{
    public static class TheReverseEngineerExtensions
    {
        public static bool WasCardPlayedFromTrash(this CardController cardController, Journal journal, Card card)
        {
			MoveCardJournalEntry moveCardJournalEntry = (from mcje in journal.MoveCardEntriesThisTurn() where mcje.Card == card && mcje.ToLocation.IsInPlayAndNotUnderCard select mcje).LastOrDefault();
			/*if (moveCardJournalEntry != null && moveCardJournalEntry.FromLocation.IsRevealed)
			{
				moveCardJournalEntry = (from mcje in journal.MoveCardEntriesThisTurn()
										where mcje.Card == card && mcje.ToLocation.IsRevealed
										select mcje).LastOrDefault();
			}*/
			return moveCardJournalEntry != null && moveCardJournalEntry.FromLocation == cardController.HeroTurnTaker.Trash;
		}
        
		public static IEnumerator PlayTopCardOfTrash(this CardController cc, TurnTakerController tt, GameController gc)
        {
			IEnumerator coroutine = gc.MoveCard(tt, tt.TurnTaker.Trash.TopCard, tt.TurnTaker.PlayArea, cardSource: cc.GetCardSource());
            if (cc.UseUnityCoroutines)
            {
                yield return gc.StartCoroutine(coroutine);
            }
            else
            {
                gc.ExhaustCoroutine(coroutine);
            }
        }
    }
}
