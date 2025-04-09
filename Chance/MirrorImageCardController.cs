using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class MirrorImageCardController : ChanceCardController
    {

		private Power _powerChosen;

		private ITrigger _makeDecisionTrigger;

		private Dictionary<Power, CardSource> _cardSources;
		public MirrorImageCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
        }

		public override void AddTriggers()
		{
			AddAdditionalPhaseActionTrigger((TurnTaker tt) => ShouldIncreasePhaseActionCount(tt), Phase.UsePower, 1);
		}

		public override IEnumerator Play()
		{
			IEnumerator coroutine = IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.TurnTaker, Phase.UsePower, 1);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
		{
			return tt == base.TurnTaker;
		}

		public override bool AskIfIncreasingCurrentPhaseActionCount()
		{
			if (base.GameController.ActiveTurnPhase.IsUsePower)
			{
				return ShouldIncreasePhaseActionCount(base.GameController.ActiveTurnTaker);
			}
			return false;
		}

        public override IEnumerator UsePower(int index = 0)
        {
			int PowerNumeralHeroes = GetPowerNumeral(0, 1);
			/*IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => tt != base.TurnTaker && tt.IsHero), SelectionType.UsePower, (TurnTaker tt) => GameController.SelectAndUsePower(FindTurnTakerController(tt).ToHero(), cardSource: GetCardSource()), numberOfTurnTakers: PowerNumeralHeroes, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}*/
			int PowerNumeralDamage = GetPowerNumeral(1, 2);
			List<DealDamageAction> damages = new List<DealDamageAction>();
			IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), PowerNumeralDamage, DamageType.Psychic, PowerNumeralHeroes, false, 0, additionalCriteria: (Card c) => IsHeroCharacterCard(c) && c != CharacterCard, storedResultsDamage: damages, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			foreach (DealDamageAction d in damages)
            {
				if (d.Target.IsHeroCharacterCard && d.DidDealDamage && !d.DidDestroyTarget)
                {
					coroutine = GameController.SelectAndUsePower(FindTurnTakerController(d.Target.Owner).ToHero(), true, cardSource: GetCardSource());
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
}
