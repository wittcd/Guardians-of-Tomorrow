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
    public class TelluricLocusCardController : CardController
    {
        public TelluricLocusCardController(Card card, TurnTakerController turnTakerController)
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
            //Whenever you play an Earth card, you may destroy an environment card.
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cep) => GameController.DoesCardContainKeyword(cep.CardEnteringPlay, "earth") && cep.CardEnteringPlay.Owner == TurnTaker && cep.CardEnteringPlay != Card, (CardEntersPlayAction cep) => GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), true, cardSource: GetCardSource()), TriggerType.DestroyCard, TriggerTiming.After);
            //When Cathain would deal himself infernal damage, you may redirect it to this card.
            AddRedirectDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.Target == CharacterCard && dd.DamageType == DamageType.Infernal, () => Card, true);
        }
    }
}
