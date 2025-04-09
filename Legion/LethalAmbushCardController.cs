using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Legion
{
    class LethalAmbushCardController : CardController
    {
        public LethalAmbushCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
        }

        public override IEnumerator Play()
        {
            /*IEnumerator coroutine = DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => c.IsHero, (Card c) => DamageBasedOnHP(c), DamageType.Energy);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }*/
            List<Card> lowestHeroes = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetsWithLowestHitPoints(1, 1, (Card c) => IsHeroTarget(c), lowestHeroes, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            /*List<SelectCardDecision> target = new List<SelectCardDecision>();
            coroutine = GameController.SelectHeroCharacterCard(DecisionMaker, SelectionType.DealDamage, target, cardSource: GetCardSource(), additionalCriteria: (Card c) => lowestHeroes.Contains(c));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }*/

            if (lowestHeroes.Count() > 0)
            {
                int dmg = DamageBasedOnHP(lowestHeroes.FirstOrDefault());
                coroutine = DealDamage(CharacterCard, lowestHeroes.FirstOrDefault(), dmg, DamageType.Energy, cardSource: GetCardSource());
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

        private int DamageBasedOnHP(Card c)
        {
            if (c.HitPoints <= 10)
            {
                return H + 2;
            }
            else
            {
                return H;
            }
        }
    }
}
