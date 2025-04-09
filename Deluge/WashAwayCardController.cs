using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Deluge
{
    class WashAwayCardController : CardController
    {
        public WashAwayCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        { 
        }

        public override IEnumerator Play()
        {
            List<DestroyCardAction> destroyed = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => (IsOngoing(c) && IsHero(c)) || IsEquipment(c), "hero ongoing or equipment"), false, destroyed, Card, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDestroyCard(destroyed) && !base.CharacterCard.IsFlipped)
            {
                foreach (DestroyCardAction dca in destroyed)
                {
                    coroutine = GameController.MoveCard(TurnTakerController, dca.CardToDestroy.Card, CharacterCard.UnderLocation, cardSource: GetCardSource());
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
            else if (DidDestroyCard(destroyed)) 
            {
                foreach (DestroyCardAction dca in destroyed)
                {
                    coroutine = DealDamage(CharacterCard, dca.CardToDestroy.CharacterCard, H - 1, DamageType.Projectile, cardSource: GetCardSource());
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
}
