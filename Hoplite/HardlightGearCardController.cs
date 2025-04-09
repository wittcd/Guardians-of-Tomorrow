using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Hoplite
{
    class HardlightGearCardController : CardController
    {
        public HardlightGearCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        //One player may play an Ongoing or Equipment card.
        //{Hoplite} deals 1 target 2 energy damage.
        public override IEnumerator Play()
        {
            LinqCardCriteria criteria = new LinqCardCriteria((Card c) => IsOngoing(c) || IsEquipment(c));
            List<SelectTurnTakerDecision> selectedHero = new List<SelectTurnTakerDecision>();
            IEnumerator coroutine = GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: GetCardSource());
            
            //IEnumerator coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.PlayCard, true, false, selectedHero, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            /*if (selectedHero.FirstOrDefault() != null)
            {
                coroutine = GameController.FindTurnTakerController()
            }*/

            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), 2, DamageType.Energy, 1, false, 1, cardSource: GetCardSource());
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
