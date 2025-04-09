using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.TheSmiler
{
    public class ArcaneTracesCardController : CardController
    {
        public ArcaneTracesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        private bool saidYes = false;

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            List<SelectTurnTakerDecision> turntakers = new List<SelectTurnTakerDecision>();
            GameController gameController = base.GameController;
            HeroTurnTakerController decisionMaker = DecisionMaker;
            Func<TurnTaker, bool> additionalCriteria = (TurnTaker tt) => IsHero(tt);
            CardSource cardSource = GetCardSource();
            IEnumerator coroutine = gameController.SelectTurnTaker(decisionMaker, SelectionType.MoveCardToPlayArea, turntakers, optional: false, allowAutoDecide: false, additionalCriteria, null, null, checkExtraTurnTakersInstead: false, canBeCancelled: true, ignoreBattleZone: false, cardSource);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            SelectTurnTakerDecision selectTurnTakerDecision = turntakers.FirstOrDefault();
            if (selectTurnTakerDecision != null && selectTurnTakerDecision.SelectedTurnTaker != null && turntakers != null)
            {
                storedResults.Add(new MoveCardDestination(selectTurnTakerDecision.SelectedTurnTaker.PlayArea, toBottom: false, showMessage: true));
                yield break;
            }
        }

        public override void AddTriggers()
        {
            //Increase infernal damage dealt to and by targets in this play area by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageType == DamageType.Infernal && (dd.Target.IsAtLocationRecursive(Card.Location) || dd.DamageSource.Card.IsAtLocationRecursive(Card.Location)), 1);
            //At the end of the hero whose play area this card is in's turn, that hero may deal 1 target 1 infernal damage. If damage is dealt this way, they deal themselves 0 infernal damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == Card.Location.OwnerTurnTaker, (PhaseChangeAction pc) => EndOfTurnDamages(), TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnDamages()
        {
            TurnTaker paOwner = Card.Location.OwnerTurnTaker;
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = FindCharacterCard(paOwner, SelectionType.HeroToDealDamage, storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidFindCard(storedResults))
            {
                DamageSource source = new DamageSource(GameController, storedResults.FirstOrDefault());
                List<DealDamageAction> storedDamage = new List<DealDamageAction>();
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, source, 1, DamageType.Infernal, 1, false, 0, storedResultsDamage: storedDamage, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidDealDamage(storedDamage))
                {
                    coroutine = GameController.DealDamage(DecisionMaker, storedResults.FirstOrDefault(), (Card c) => c == storedResults.FirstOrDefault(), 0, DamageType.Infernal, cardSource: GetCardSource());
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
