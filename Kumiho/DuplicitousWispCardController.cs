using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Kumiho
{
    class DuplicitousWispCardController : CardController
    {
		public DuplicitousWispCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			SpecialStringMaker.ShowHeroWithMostCards(false);
		}

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageAndMaybeMoveResponse, TriggerType.DealDamage);
            base.AddTriggers();
        }

        private IEnumerator DealDamageAndMaybeMoveResponse(PhaseChangeAction pc)
        {
            List<TurnTaker> stored = new List<TurnTaker>();
            IEnumerator coroutine = FindHeroWithMostCardsInPlay(stored);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = DealDamage(base.Card, (Card c) => c.Location == stored.FirstOrDefault().PlayArea && c.IsTarget, 3, DamageType.Radiant);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //if Stolen Soul is in play
            List<Card> findSoul = FindCardsWhere((Card c) => c.Identifier == "StolenSoul" && c.IsInPlayAndHasGameText).ToList();
            if (findSoul.Count() > 0)
            {
                Card soul = findSoul.FirstOrDefault();
                coroutine = GameController.MoveCard(FindTurnTakerController(base.TurnTaker), base.Card, soul.UnderLocation, showMessage: true, cardSource: GetCardSource());
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
                LinqCardCriteria heroNonChar = new LinqCardCriteria((Card c) => !c.IsCharacter && IsHero(c));
                coroutine = GameController.SelectAndDestroyCards(DecisionMaker, heroNonChar, H, cardSource: GetCardSource());
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
}
