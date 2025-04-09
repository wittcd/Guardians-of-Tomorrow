using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Heartbeat
{
    class HeartbeatCharacterCardController : HeroCharacterCardController
    {

        public HeartbeatCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowLocationOfCards(new LinqCardCriteria((Card c) => c.Identifier == "ArcaneCharge", "Arcane Charge", useCardsSuffix: false), reverseOrder: false, null, null, specifyPlayAreas: true).Condition = () => !base.Card.IsIncapacitatedOrOutOfGame;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<Card> charges = (List<Card>)FindCardsWhere((Card c) => c.Identifier == "ArcaneCharge" && (c.Location == base.TurnTaker.Deck || c.Location == base.TurnTaker.Trash));
            if (charges.Count > 0)
            {
                IEnumerator coroutine = SearchForCards(DecisionMaker, true, true, 1, 1, new LinqCardCriteria((Card c) => c.Identifier == "ArcaneCharge"), true, false, false, false, null, false, null, null);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                IEnumerator coroutine = GameController.SendMessageAction("There are no Arcane Charges in your deck or trash", Priority.Medium, GetCardSource());
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //play
                        IEnumerator coroutine = GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        yield break;
                    }
                case 1:
                    {
                        //power
                        IEnumerator coroutine = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        yield break;
                    }
                case 2:
                    {
                        IEnumerator coroutine = GameController.SelectHeroAndIncreaseNextDamageDealt(DecisionMaker, 2, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        yield break;
                    }
            }
        }
    }
}
