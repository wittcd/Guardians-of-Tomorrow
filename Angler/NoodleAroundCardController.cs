using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Angler
{
    public class NoodleAroundCardController : CardController
    {
        public NoodleAroundCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = GameController.DrawCardsEx(new LinqTurnTakerCriteria((TurnTaker tt) => tt == TurnTaker), 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            SelectCardDecision options = new SelectCardDecision(GameController, DecisionMaker, SelectionType.DealDamage, FindCardsWhere((Card c) => c.IsTarget && !IsHero(c) && c.IsInPlayAndHasGameText));
            coroutine = GameController.SelectCardAndDoAction(options, (SelectCardDecision sc) => GameController.DealDamage(DecisionMaker, sc.SelectedCard, (Card c) => c == CharacterCard, 1, DamageType.Melee, true, cardSource: GetCardSource()));
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
