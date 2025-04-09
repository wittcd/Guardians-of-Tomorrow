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
    public class SSLangleyCardController : CardController
    {
        public SSLangleyCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public static readonly string FirstBaitDestroyKey = "AnglerFirstBaitDestroyed";

        public override void AddTriggers()
        {
            AddRedirectDamageTrigger((DealDamageAction dd) => dd.Target == CharacterCard && (dd.DamageSource.Card != Card), () => Card, true);
            AddTrigger<DestroyCardAction>((DestroyCardAction dc) => dc.CardToDestroy.Card.DoKeywordsContain("bait") && !GameController.GetCardPropertyJournalEntryBoolean(Card, FirstBaitDestroyKey).Value, (DestroyCardAction dc) => DrawAndSetProperty(), TriggerType.DrawCard, TriggerTiming.After);
            AddStartOfTurnTrigger((TurnTaker tt) => true, (PhaseChangeAction pc) => reset_bait_destroyed(), TriggerType.Other);
            AddWhenDestroyedTrigger((DestroyCardAction dc) => DamageHeroWhenDestroyed(), TriggerType.DealDamage);
        }

        private IEnumerator DamageHeroWhenDestroyed()
        {
            List<DealDamageAction> damages = new List<DealDamageAction>();
            damages.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, Card), CharacterCard, 3, DamageType.Cold, true));
            damages.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, Card), CharacterCard, 3, DamageType.Psychic, true));
            IEnumerator coroutine = DealMultipleInstancesOfDamage(damages, (Card c) => c == CharacterCard);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DrawAndSetProperty()
        {
            GameController.AddCardPropertyJournalEntry(Card, FirstBaitDestroyKey, true);
            IEnumerator coroutine = GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator reset_bait_destroyed()
        {
            GameController.AddCardPropertyJournalEntry(Card, FirstBaitDestroyKey, false);
            yield break;
        }
    }
}
