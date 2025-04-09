using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Price
{
    class CautionToTheWindsCardController : CardController
    {
        public CautionToTheWindsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController) { }

        public override IEnumerator Play()
        {

            List<SelectNumberDecision> numbers = new List<SelectNumberDecision>();
            List<DrawCardAction> draws = new List<DrawCardAction>();
            IEnumerator coroutine = GameController.SelectNumberEx(DecisionMaker, SelectionType.DrawCard, 0, 3, numbers, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.DrawCardsEx(new LinqTurnTakerCriteria((TurnTaker tt) => tt == TurnTaker), numbers.FirstOrDefault().SelectedNumber.Value, storedResults: draws, cardSource: GetCardSource()); ;
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            int n = GetNumberOfCardsDrawn(draws);

            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 3, DamageType.Infernal, n, false, 0, cardSource: GetCardSource());
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
