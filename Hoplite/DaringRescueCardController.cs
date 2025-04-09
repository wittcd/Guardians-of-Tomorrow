using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Hoplite
{
    class DaringRescueCardController : CardController
    {
        public DaringRescueCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<SelectCardDecision> selectCards = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.ReduceNextDamageTaken, new LinqCardCriteria((Card c) => IsHeroCharacterCard(c) && c != base.CharacterCard, "Other hero character card"), selectCards, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (selectCards.FirstOrDefault() != null)
            {
                Card c = selectCards.FirstOrDefault().SelectedCard;

                ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(2);
                rdse.TargetCriteria.IsSpecificCard = c;
                rdse.NumberOfUses = 1;
                coroutine = GameController.AddStatusEffect(rdse, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            ReduceDamageStatusEffect selfReduce = new ReduceDamageStatusEffect(1);
            selfReduce.TargetCriteria.IsSpecificCard = base.CharacterCard;
            selfReduce.NumberOfUses = 1;
            coroutine = GameController.AddStatusEffect(selfReduce, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), 3, DamageType.Melee, 1, false, 0, cardSource: GetCardSource());
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
