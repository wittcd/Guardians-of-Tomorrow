using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Starstone
{
    public class GuardiansOfTomorrowStarstoneCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public GuardiansOfTomorrowStarstoneCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
			//1 self melee damage, if damage taken, play stone from trash
			List<DealDamageAction> storedDamage = new List<DealDamageAction>();
			int powerNumeral = GetPowerNumeral(0, 1);
			IEnumerator coroutine = DealDamage(base.Card, base.Card, powerNumeral, DamageType.Melee, isIrreducible: false, optional: false, isCounterDamage: false, null, storedDamage);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (!DidDealDamage(storedDamage, base.Card))
			{
				yield break;
			}
			//play stone from trash
			if (base.TurnTaker.IsHero)
			{
				MoveCardDestination obj = new MoveCardDestination(DecisionMaker.TurnTaker.PlayArea);
				IEnumerator coroutine3 = base.GameController.SelectCardFromLocationAndMoveIt(DecisionMaker, DecisionMaker.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("stone", false, false), "stone"), obj.ToEnumerable(), isPutIntoPlay: true, playIfMovingToPlayArea: true, shuffleAfterwards: false, optional: false, null, flipFaceDown: false, showOutput: false, null, isDiscardIfMovingtoTrash: false, allowAutoDecide: false, null, null, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine3);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine3);
				}
			}
			else
			{
				IEnumerator coroutine3 = base.GameController.SendMessageAction(base.Card.Title + " has no trash to move cards from.", Priority.Medium, GetCardSource(), null, showCardSource: true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine3);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine3);
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
						IEnumerator coroutine3 = gameController2.SelectHeroToDrawCard(heroTurnTakerController, optionalSelectHero: false, optionalDrawCard: true, allowAutoDecideHero: false, null, null, null, cardSource);
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
						IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.MoveCardOnDeck, new LinqCardCriteria((Card c) => IsVillain(c) && c.IsInPlay && !c.IsCharacter && !base.GameController.IsCardIndestructible(c) && !c.IsOneShot && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), "non-indestructible non-character villain cards in play", useCardsSuffix: false), storedResults, optional: false, allowAutoDecide: false, null, includeRealCardsOnly: true, GetCardSource());
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
								string text = (IsVillain(base.TurnTaker) ? "hero" : "villain");
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
							IEnumerator coroutine3 = gameController.MoveCard(turnTakerController, card, nativeDeck, toBottom: false, isPutIntoPlay: false, playCardIfMovingToPlayArea: true, null, showMessage: false, null, null, null, evenIfIndestructible: false, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, cardSource);
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