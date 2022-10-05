using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Kumiho
{
    class RaveningCardController : CardController
    {
        public RaveningCardController(Card card, TurnTakerController turnTakerController)
               : base(card, turnTakerController)
        {
            
        }

        public override IEnumerator Play()
        {
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.UntilEndOfNextTurn(base.TurnTaker);
            idse.SourceCriteria.IsVillain = true;
            IEnumerator coroutine = AddStatusEffect(idse, true);
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
                Card relevantHero = soul.Location.OwnerTurnTaker.CharacterCard;

                IncreaseDamageStatusEffect idse2 = new IncreaseDamageStatusEffect(1);
                idse2.SourceCriteria.IsSpecificCard = relevantHero;
                coroutine = AddStatusEffect(idse2, true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }


                coroutine = DealDamage(relevantHero, relevantHero, 1, DamageType.Toxic, true, cardSource: GetCardSource());
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
                idse.SourceCriteria.IsVillain = true;
                coroutine = DoNothing();
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
