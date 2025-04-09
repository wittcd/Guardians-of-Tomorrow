using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace GuardiansOfTomorrow.Angler
{
    public class AnglerCharacterCardController : HeroCharacterCardController
    {
        public AnglerCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //You may play a Bait card or draw a card.
            List<Function> options = new List<Function>();
            options.Add(new Function(DecisionMaker, "Play a Bait Card", SelectionType.PlayCard, () => GameController.SelectAndPlayCardFromHand(DecisionMaker, false, null, new LinqCardCriteria((Card c) => c.DoKeywordsContain("bait"), "bait"), cardSource: GetCardSource()), FindCardsWhere((Card c) => c.DoKeywordsContain("bait") && c.Location == TurnTaker.ToHero().Hand).Count() > 0));
            options.Add(new Function(DecisionMaker, "Draw a Card", SelectionType.DrawCard, () => GameController.DrawCard(TurnTaker.ToHero(), cardSource: GetCardSource())));
            SelectFunctionDecision select = new SelectFunctionDecision(GameController, DecisionMaker, options, false, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(select);
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
            switch (index)
            {
                case 0:
                    {
                        // One player may draw a card now.
                        IEnumerator coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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
                        // One hero may use a power
                        IEnumerator coroutine = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
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
                        // You may destroy a hero non-character card. If you do, that card's owner deals 1 target 3 projectile damage.
                        List<DestroyCardAction> destroys = new List<DestroyCardAction>();
                        IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsHero(c) && !c.IsCharacter, "hero non-character"), true, destroys, null, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (DidDestroyCard(destroys))
                        {
                            TurnTaker owner = destroys.FirstOrDefault().CardToDestroy.Card.Owner;
                            List<Card> characterCards = new List<Card>();
                            coroutine = FindCharacterCard(owner, SelectionType.DealDamage, characterCards);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            if (characterCards.Count > 0) {
                                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, characterCards.FirstOrDefault()), 3, DamageType.Projectile, 1, false, 0, cardSource: GetCardSource());
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
                        break;
                    }
            }
        }
    }
}
