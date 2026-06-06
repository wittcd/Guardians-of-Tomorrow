using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Cathain
{
    public class VolcanicLocusCardController : CardController
    {
        public VolcanicLocusCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //Destroy your other Loci
            IEnumerator coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c != Card && c.Owner == TurnTaker && GameController.DoesCardContainKeyword(c, "locus"), "locus"), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectAndPlayCardFromHand(TurnTakerController.ToHero(), true, cardSource: GetCardSource());
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
            //Whenever you play a Fire card, this card deals each non-hero target 1 fire damage.
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cep) => GameController.DoesCardContainKeyword(cep.CardEnteringPlay, "fire") && cep.CardEnteringPlay.Owner == TurnTaker && cep.CardEnteringPlay != Card, (CardEntersPlayAction cep) => GameController.DealDamage(DecisionMaker, Card, (Card c) => !IsHeroTarget(c), 1, DamageType.Fire, cardSource: GetCardSource()), TriggerType.DealDamage, TriggerTiming.After);
            //When Cathain would deal himself infernal damage, you may redirect it to this card.
            AddRedirectDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.Target == CharacterCard && dd.DamageType == DamageType.Infernal, () => Card, true);
        }
    }
}
