using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using Handelabra.Sentinels.Engine;

namespace GuardiansOfTomorrow.TheSmiler
{
    public class TheKnifeOfSilentScreamsCardController : CardController
    {
        public TheKnifeOfSilentScreamsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => EndTurnResponse(), TriggerType.DealDamage);
        }

        private IEnumerator EndTurnResponse()
        {
            IEnumerator coroutine = DealDamageToLowestHPEx(CharacterCard, 1, (Card c) => !IsVillain(c), (Card c) => H - 1, DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<TurnTaker> cluedHero = new List<TurnTaker>();
            coroutine = FindHeroWithMostCardsInPlayArea(cluedHero, cardCriteria: new LinqCardCriteria((Card c) => c.IsClue, "clue"));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<DealDamageAction> damages = new List<DealDamageAction>();
            damages.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 1, DamageType.Sonic));
            damages.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 1, DamageType.Psychic));
            coroutine = SelectTargetsAndDealMultipleInstancesOfDamage(damages, (Card c) => cluedHero.Contains(c.Owner) && IsHeroCharacterCard(c), null, 1, 1);
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
