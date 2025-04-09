using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Arsenal
{
    class SharurCardController : ArsenalRelicCardController
    {
        public SharurCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = ReturnAllButTwoRelicsToHand();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.DamageType == DamageType.Projectile, (DealDamageAction dd) => 1, false);
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.DamageType == DamageType.Projectile && GetNumberOfProjectileDealtThisTurn() <= 1, RevealAndReplaceACard, TriggerType.RevealCard, TriggerTiming.After);
        }

        private IEnumerator RevealAndReplaceACard(DealDamageAction dd)
        {
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            IEnumerator coroutine = GameController.SelectADeck(DecisionMaker, SelectionType.RevealTopCardOfDeck, (Location d) => true, storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectDeck(storedResults))
            {
                Location deck = storedResults.FirstOrDefault().SelectedLocation.Location;
                coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(DecisionMaker, deck, false, false, false, new LinqCardCriteria((Card c) => false), 1, 1, false, false, RevealedCardDisplay.ShowRevealedCards);
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
