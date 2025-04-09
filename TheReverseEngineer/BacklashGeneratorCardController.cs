using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using GuardiansOfTomorrow.TheReverseEngineer;

namespace GuardiansOfTomorrow.TheReverseEngineer
{
    class BacklashGeneratorCardController : TheReverseEngineerCardController
    {
        public BacklashGeneratorCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            
        }

        public override IEnumerator FromTrashAction()
        {
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

        public override void AddTriggers()
        {
            AddTrigger(AddCounterDamageTrigger((DealDamageAction dd) => dd.Target == CharacterCard && dd.DidDealDamage && !IsHero(dd.DamageSource.Card), () => CharacterCard, () => CharacterCard, true, 2, DamageType.Lightning));
        }
    }
}
