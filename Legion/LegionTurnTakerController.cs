using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Legion
{
    class LegionTurnTakerController : TurnTakerController
    {
        public LegionTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
            foreach (Card c in base.TurnTaker.Deck.Cards.Where((Card c) => c.Identifier == "HackedVaultDoor" && c.MaximumHitPoints != 2 * H))
            {
                c.SetMaximumHP(2 * H, !c.IsInPlayAndHasGameText);
            }
        }

        public override IEnumerator StartGame()
        {
            int SwarmsToMove = H;
            IEnumerator coroutine = GameController.SetHP(base.CharacterCard, H * 10, CharacterCardController.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<MoveCardDestination> locs = new List<MoveCardDestination>();
            locs.Add(new MoveCardDestination(base.CharacterCard.UnderLocation));
            IEnumerator coroutine2 = GameController.SelectCardsFromLocationAndMoveThem(FindDecisionMaker(), GameController.GetNativeDeck(base.CharacterCard), SwarmsToMove, SwarmsToMove, new LinqCardCriteria((Card c) => c.DoKeywordsContain("nanite swarm"), "nanite swarm"), locs, allowAutoDecide: true, selectionType: SelectionType.MoveCardToUnderCard, cardSource: base.CharacterCardController.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }

            
            if (!CharacterCardController.IsGameChallenge)
            {
                coroutine2 = GameController.MoveCards(this, FindCardsWhere((Card c) => c.DoKeywordsContain("nanite swarm") && c.Location == TurnTaker.Deck), TurnTaker.OutOfGame, cardSource: CharacterCardController.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
        }
    }
}
