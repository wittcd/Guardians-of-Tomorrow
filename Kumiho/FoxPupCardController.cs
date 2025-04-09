using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Kumiho
{
	class FoxPupCardController : CardController
	{
		public FoxPupCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			SpecialStringMaker.ShowHeroTargetWithLowestHP(2, 1, false);
		}

        public override void AddTriggers()
        {
			AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealMultipleDamageMaybeDiscardResponse, TriggerType.DealDamage);
            base.AddTriggers();
        }

		private IEnumerator DealMultipleDamageMaybeDiscardResponse(PhaseChangeAction pc)
        {
			List<DealDamageAction> damagesToDeal = new List<DealDamageAction>();
			damagesToDeal.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, base.Card), null, 1, DamageType.Melee));
			damagesToDeal.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, base.Card), null, 1, DamageType.Fire));
			damagesToDeal.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, base.Card), null, 1, DamageType.Infernal));
			IEnumerator coroutine = DealMultipleInstancesOfDamageToHighestLowestHP(damagesToDeal, (Card c) => IsHero(c), HighestLowestHP.LowestHP, 2, 1);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//if Stolen Soul is in play
			List<Card> findSoul = FindCardsWhere((Card c) => c.Identifier == "StolenSoul" && c.IsInPlayAndHasGameText).ToList();
			if (findSoul.Count() > 0)
			{
				Card soul = findSoul.FirstOrDefault();
				coroutine = GameController.SelectAndDiscardCard(FindTurnTakerController(soul.Location.OwnerTurnTaker).ToHero(), cardSource: GetCardSource());
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
