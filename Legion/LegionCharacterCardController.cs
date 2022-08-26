using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Legion
{
    class LegionCharacterCardController : VillainCharacterCardController
    {
        public LegionCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                this.SideTriggers.Add(AddEndOfTurnTrigger(tt => tt == this.TurnTaker, EndVillainTurnResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroyCard }));
            } else
            {

            }
        }

        private IEnumerator EndVillainTurnResponse(PhaseChangeAction pc) {
            List<TurnTaker> heroMost = new List<TurnTaker>();
            LinqCardCriteria e = new LinqCardCriteria((Card c) => IsEquipment(c), "equipment");
            IEnumerator coroutine = FindHeroWithMostCardsInPlay(heroMost, 1, 1, null, e, true, SelectionType.MostCardsInPlay);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (heroMost.FirstOrDefault() != null)
            {
                TurnTaker target = heroMost.FirstOrDefault();
                coroutine = FindCharacterCardToTakeDamage(target, null, base.CharacterCard, H - 2, DamageType.Melee, false);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                LinqCardCriteria targetEquip = new LinqCardCriteria((Card c) => IsEquipment(c) && c.Owner == target, "equipment");
                coroutine = GameController.SelectAndDestroyCard(FindTurnTakerController(target).ToHero(), targetEquip, false, null, base.CharacterCard, GetCardSource());
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
