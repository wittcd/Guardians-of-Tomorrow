using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace GuardiansOfTomorrow.Deluge
{
    class SeaSerpentsCardController : CardController
    {
        public SeaSerpentsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(1, () => H - 2, new LinqCardCriteria((Card c) => !c.IsVillain));
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == Card && dd.Target.IsEnvironment, 2);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DamageLowests(), TriggerType.DealDamage);
        }

        private IEnumerator DamageLowests()
        {
            List<DealDamageAction> damageList = new List<DealDamageAction>();
            damageList.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, Card), null, 1, DamageType.Melee));
            damageList.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, Card), null, 1, DamageType.Cold));
            IEnumerator coroutine = DealMultipleInstancesOfDamageToHighestLowestHP(damageList, (Card c) => !c.IsVillain, HighestLowestHP.LowestHP, 1, H - 2);
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
