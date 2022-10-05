using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Kumiho
{
    class SeedsOfDoubtCardController : CardController
    {
        public SeedsOfDoubtCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            List<DealDamageAction> damages = new List<DealDamageAction>();
            damages.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, base.CharacterCard), null, 1, DamageType.Toxic));
            damages.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, base.CharacterCard), null, 1, DamageType.Psychic));

            List<TurnTaker> fewest = new List<TurnTaker>();
            IEnumerator coroutine = FindHeroWithFewestCardsInPlay(fewest);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DealMultipleInstancesOfDamage(damages, (Card c) => c == fewest.FirstOrDefault().CharacterCard);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

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
                coroutine = PlayTheTopCardOfTheVillainDeckResponse(null);
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
