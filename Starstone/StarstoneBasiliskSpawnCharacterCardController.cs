using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Starstone
{
    class StarstoneBasiliskSpawnCharacterCardController : HeroCharacterCardController
    {
        public StarstoneBasiliskSpawnCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.ModifiesKeywords);
        }

        public override bool AskIfCardContainsKeyword(Card card, string keyword, bool evenIfUnderCard = false, bool evenIfFaceDown = false)
        {
            if (keyword == "stone" && GameController.StatusEffectManager.StatusEffectControllers.Any((StatusEffectController sec) => sec.StatusEffect.Identifier == "AddStone" && sec.StatusEffect.TargetLeavesPlayExpiryCriteria.Card == card) && card.BattleZone == base.CardWithoutReplacements.BattleZone)
            {
                return true;
            }

            return base.AskIfCardContainsKeyword(card, keyword, evenIfUnderCard, evenIfFaceDown);
        }

		public override IEnumerable<string> AskForCardAdditionalKeywords(Card card)
		{
			if (GameController.StatusEffectManager.StatusEffectControllers.Any((StatusEffectController sec) => sec.StatusEffect.Identifier == "AddStone" && sec.StatusEffect.TargetLeavesPlayExpiryCriteria.Card == card) && card.BattleZone == base.CardWithoutReplacements.BattleZone)
			{
				return new string[1] { "stone" };
			}
			return base.AskForCardAdditionalKeywords(card);
		}

		public override IEnumerator UsePower(int index = 0)
        {
            int PowerNumeralTargets = GetPowerNumeral(0, 1);
            int PowerNumeralDamage = GetPowerNumeral(1, 1);

            List<DealDamageAction> store = new List<DealDamageAction>();

            /*PhaseChangeStatusEffect AddStone = new PhaseChangeStatusEffect();
            AddStone.Identifier = "AddStone";
            AddStone.UntilEndOfNextTurn(base.TurnTaker);*/


            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), PowerNumeralDamage, DamageType.Toxic, PowerNumeralTargets, false, PowerNumeralTargets, storedResultsDamage: store, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(store))
            {
                foreach(DealDamageAction dd in store)
                {
                    PhaseChangeStatusEffect AddStone = new PhaseChangeStatusEffect();
                    AddStone.Identifier = "AddStone";
                    AddStone.UntilEndOfNextTurn(base.TurnTaker);
                    if (dd.DidDealDamage && !dd.DidDestroyTarget && !dd.Target.IsCharacter && dd.TargetHitPointsAfterBeingDealtDamage < 5)
                    {
                        AddStone.UntilTargetLeavesPlay(dd.Target);
						coroutine = AddStatusEffect(AddStone, false);
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

		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						GameController gameController2 = base.GameController;
						HeroTurnTakerController heroTurnTakerController = base.HeroTurnTakerController;
						CardSource cardSource = GetCardSource();
						IEnumerator coroutine3 = gameController2.SelectHeroToPlayCard(DecisionMaker, cardSource: GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine3);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine3);
						}
						break;
					}
				case 1:
					{
						GameController gameController = base.GameController;
						HeroTurnTakerController heroTurnTakerController = base.HeroTurnTakerController;
						LinqCardCriteria cardCriteria = new LinqCardCriteria((Card c) => c.IsEnvironment, "environment");
						int? numberOfCards = 1;
						int? requiredDecisions = 1;
						CardSource cardSource = GetCardSource();
						IEnumerator coroutine = gameController.SelectAndDestroyCards(heroTurnTakerController, cardCriteria, numberOfCards, optional: false, requiredDecisions, null, null, null, ignoreBattleZone: false, null, null, null, cardSource);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
				case 2:
					{
						List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
						IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.MoveCardOnDeck, new LinqCardCriteria((Card c) => c.IsInPlay && !c.IsCharacter && !base.GameController.IsCardIndestructible(c) && c.HitPoints <= 3 && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), "non-indestructible non-character target with 3 or less HP", useCardsSuffix: false), storedResults, optional: false, allowAutoDecide: false, null, includeRealCardsOnly: true, GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						SelectCardDecision selectCardDecision = storedResults.Where((SelectCardDecision d) => d.Completed).FirstOrDefault();
						if (selectCardDecision != null && selectCardDecision.SelectedCard != null)
						{
							Card card = selectCardDecision.SelectedCard;
							if (selectCardDecision.Choices.Count() == 1)
							{
								string text = ("non-character target with less than 3 HP");
								IEnumerator coroutine2 = base.GameController.SendMessageAction(card.Title + " is the only non-indestructible " + text + " card in play.", Priority.Low, GetCardSource(), selectCardDecision.Choices, showCardSource: true);
								if (base.UseUnityCoroutines)
								{
									yield return base.GameController.StartCoroutine(coroutine2);
								}
								else
								{
									base.GameController.ExhaustCoroutine(coroutine2);
								}
							}
							GameController gameController = base.GameController;
							TurnTakerController turnTakerController = base.TurnTakerController;
							Location nativeDeck = card.NativeDeck;
							CardSource cardSource = GetCardSource();
							IEnumerator coroutine3 = gameController.ShuffleCardIntoLocation(DecisionMaker, card, nativeDeck, false, cardSource: GetCardSource());
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine3);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine3);
							}
						}
						break;
					}
			}
		}
	}
}
