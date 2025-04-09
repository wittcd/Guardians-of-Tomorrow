using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.Ludramor
{
    class ChiefEngineerKomacCardController : CardController
    {
        public ChiefEngineerKomacCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("technician"), "technician"));
            SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => MostDamagedSpaceship().Contains(c), "most damaged spaceships", false));
        }

        private IEnumerator RedirectToTechnician(DealDamageAction dd)
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card card) => card != Card && card.DoKeywordsContain("technician"), storedResults, dd, null, evenIfCannotDealDamage: false, optional: false, null, ignoreBattleZone: false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card2 = storedResults.FirstOrDefault();
            if (card2 != null)
            {
                IEnumerator coroutine2 = base.GameController.RedirectDamage(dd, card2, isOptional: false, GetCardSource());
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

        private int numTechnicians()
        {
            return FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("technician")).Count();
        }

        public override void AddTriggers()
        {
            AddTrigger((GainHPAction gh) => gh.HpGainer.DoKeywordsContain("spaceship"), (GainHPAction gh) => GameController.IncreaseHPGain(gh, numTechnicians(), GetCardSource()), TriggerType.IncreaseHPGain, TriggerTiming.Before);
            AddTrigger((DealDamageAction dd) => dd.Target == Card, RedirectToTechnician, TriggerType.RedirectDamage, TriggerTiming.Before);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => HealMostDamagedSpaceship(), TriggerType.GainHP);
        }

        private List<Card> MostDamagedSpaceship()
        {
            List<Card> spaceships = (List<Card>)FindCardsWhere((Card c) => c.IsTarget && IsVillain(c) && c.DoKeywordsContain("spaceship") && c.IsInPlayAndHasGameText);
            List<Card> damaged = new List<Card>();
            int damage = -1;
            foreach (Card spaceship in spaceships)
            {
                if (spaceship.MaximumHitPoints - spaceship.HitPoints > damage)
                {
                    damaged.Clear(); //remove previous record holders
                    damage = (spaceship.MaximumHitPoints - spaceship.HitPoints).Value;
                    damaged.Add(spaceship);
                }
                else if (spaceship.MaximumHitPoints - spaceship.HitPoints == damage) {
                    damaged.Add(spaceship);
                }
            }
            return damaged;
        }

        private IEnumerator HealMostDamagedSpaceship()
        {
            List<Card> mostdamaged = MostDamagedSpaceship();
            IEnumerator coroutine = GameController.SelectAndGainHP(DecisionMaker, H - 1, false, (Card c) => mostdamaged.Contains(c), cardSource: GetCardSource());
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
