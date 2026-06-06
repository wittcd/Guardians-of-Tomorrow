using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace GuardiansOfTomorrow.Cathain
{
    public class AethericLocusCardController : CardController
    {
        public AethericLocusCardController(Card card, TurnTakerController turnTakerController)
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
            //Whenever you play an Air card, this card deals 1 target 2 lightning damage.
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cep) => cep.CardEnteringPlay.Owner == TurnTaker && GameController.DoesCardContainKeyword(cep.CardEnteringPlay, "air") && cep.CardEnteringPlay != Card, (CardEntersPlayAction cep) => GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), 2, DamageType.Lightning, 1, false, 1, cardSource: GetCardSource()), TriggerType.DealDamage, TriggerTiming.After);
            //When Cathain would deal himself infernal damage, you may redirect it to this card.
            AddRedirectDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.Target == CharacterCard && dd.DamageType == DamageType.Infernal, () => Card, true);
        }
    }
}
