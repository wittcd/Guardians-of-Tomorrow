using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using GuardiansOfTomorrow.TheReverseEngineer;

namespace GuardiansOfTomorrow.TheReverseEngineer
{
    class TheReverseEngineerCharacterCardController : HeroCharacterCardController
    {
        public TheReverseEngineerCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.Owner == TurnTaker && c.DoKeywordsContain("salvage"), "salvage"));
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine = GameController.DiscardTopCards(DecisionMaker, TurnTaker.Deck, 1, null, null, true, null, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = TheReverseEngineerExtensions.PlayTopCardOfTrash(this, TurnTakerController, GameController);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch(index)
            {
                case 0:
                    {
                        //use power
                        IEnumerator coroutine = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, GetCardSource());
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
                case 1:
                    {
                        //play card
                        IEnumerator coroutine2 = GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //play top card of a trash WARNING BUGGED CURRENTLY FIX LATER

                        IEnumerator coroutine3 = GameController.SelectCardAndDoAction(new SelectCardDecision(GameController, DecisionMaker, SelectionType.MoveCard, FindCardsWhere((Card c) => c.Location.IsTrash && c.Location.IsHero && c == c.Location.TopCard), cardSource: GetCardSource()), PlaySelectedCardFromTrash);
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
            }
        }

        private IEnumerator PlaySelectedCardFromTrash(SelectCardDecision scd)
        {
            IEnumerator coroutine = GameController.MoveCard(TurnTakerController, scd.SelectedCard, scd.SelectedCard.Owner.PlayArea, cardSource: GetCardSource());
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
