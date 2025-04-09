using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace GuardiansOfTomorrow.Argus
{
    public class CommandDroneCardController : CamdroneCardController
    {
        public CommandDroneCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override IEnumerator ActivateOperate()
        {
            LinqTurnTakerCriteria addcrit = new LinqTurnTakerCriteria((TurnTaker t) => t != base.TurnTaker);
            IEnumerator coroutine = GameController.SelectHeroToUsePower(DecisionMaker, additionalCriteria: addcrit, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeralOperates = GetPowerNumeral(0, 3);
            List<Card> usedCards = new List<Card>();
            LinqCardCriteria unused = new LinqCardCriteria((Card c) => !usedCards.Contains(c) && c.IsInPlay);
            for (int x = 0; x < powerNumeralOperates; x++) 
            {
                List<ActivateAbilityDecision> activated = new List<ActivateAbilityDecision>();
                IEnumerator coroutine = GameController.SelectAndActivateAbility(DecisionMaker, "operate", unused, activated, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (activated.FirstOrDefault() != null)
                {
                    usedCards.Add(activated.FirstOrDefault().SelectedCard);
                }
                else
                {
                    x = powerNumeralOperates;
                }
            }
        }
    }
}
