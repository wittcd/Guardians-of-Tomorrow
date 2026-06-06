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
    public class OceanicLocusCardController : CardController
    {
        public OceanicLocusCardController(Card card, TurnTakerController turnTakerController)
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
            //Whenever you play a Water card, Cathain gains 1 HP
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cep) => cep.CardEnteringPlay.Owner == TurnTaker && GameController.DoesCardContainKeyword(cep.CardEnteringPlay, "water") && cep.CardEnteringPlay != Card, (CardEntersPlayAction cep) => GameController.GainHPEx(CharacterCard, 1, cardSource: GetCardSource()), TriggerType.GainHP, TriggerTiming.After);
            //When Cathain would deal himself infernal damage, you may redirect it to this card.
            AddRedirectDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == CharacterCard && dd.Target == CharacterCard && dd.DamageType == DamageType.Infernal, () => Card, true);
        }
    }
}
