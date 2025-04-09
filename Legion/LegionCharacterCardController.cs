using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Legion
{
    class LegionCharacterCardController : VillainCharacterCardController
    {
        public LegionCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            AllowFastCoroutinesDuringPretend = false;
            SpecialStringMaker.ShowHeroWithMostCards(false, new LinqCardCriteria((Card c) => IsEquipment(c), "equipment"), () => !base.Card.IsFlipped);
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.Card.UnderLocation);
        }

        public override void AddTriggers()
        {
            //base.AddTriggers();
            AddDefeatedIfDestroyedTriggers(false);
            base.AddTriggers();
        }

        public override void AddSideTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                AddSideTrigger(AddEndOfTurnTrigger(tt => tt == this.TurnTaker, EndVillainTurnResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroyCard }));
                AddSideTrigger(AddTrigger((PlayCardAction pc) => pc.CardToPlay.DoKeywordsContain("nanite swarm"), MoveNaniteSwarmUnderResponse, TriggerType.MoveCard, TriggerTiming.After));
                if (IsGameAdvanced)
                {
                    AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card.IsVillain, 1));
                }
                AddSideTrigger(AddTrigger((DealDamageAction dd) => dd.DidDealDamage && dd.Target == base.CharacterCard && dd.TargetHitPointsAfterBeingDealtDamage <= 0 && FindCardsWhere((Card c) => c.Location == base.Card.UnderLocation).Count() > 0, FlipThisCharacterCardResponse, TriggerType.FlipCard, TriggerTiming.After));
            } 
            else
            {
                if (IsGameAdvanced)
                {
                    AddSideTrigger(AddReduceDamageTrigger((Card c) => c.IsVillain, 1));
                }
                AddSideTrigger(AddTrigger((DestroyCardAction dc) => dc.CardToDestroy.Card.DoKeywordsContain("nanite swarm") && dc.WasCardDestroyed, (DestroyCardAction dc) => RemoveDestroyedSwarmThenFlipResponse(dc), TriggerType.FlipCard, TriggerTiming.After));
            }
        }

        private IEnumerator RemoveDestroyedSwarmThenFlipResponse(DestroyCardAction dc)
        {
            if (!IsGameChallenge)
            {
                dc.SetPostDestroyDestination(TurnTaker.OutOfGame);
            }

            IEnumerator coroutine = FlipThisCharacterCardResponse(dc);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator EndVillainTurnResponse(PhaseChangeAction pc) {
            List<TurnTaker> heroMost = new List<TurnTaker>();
            LinqCardCriteria e = new LinqCardCriteria((Card c) => IsEquipment(c), "equipment");
            IEnumerator coroutine = FindHeroWithMostCardsInPlay(heroMost, 1, 1, null, e, true, SelectionType.MostCardsInPlay);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (heroMost.FirstOrDefault() != null)
            {
                TurnTaker target = heroMost.FirstOrDefault();
                List<DealDamageAction> ddlist = new List<DealDamageAction>();
                coroutine = DealDamage(base.CharacterCard, target.CharacterCard, H - 1, DamageType.Melee, storedResults: ddlist, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (DidDealDamage(ddlist) && IsHeroCharacterCard(ddlist.FirstOrDefault().Target))
                {
                    LinqCardCriteria targetEquip = new LinqCardCriteria((Card c) => IsEquipment(c) && c.Owner == ddlist.FirstOrDefault().Target.Owner, "equipment");
                    coroutine = GameController.SelectAndDestroyCard(FindTurnTakerController(target).ToHero(), targetEquip, false, null, base.CharacterCard, GetCardSource());
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

        private IEnumerator MoveNaniteSwarmUnderResponse(PlayCardAction pc)
        {
            IEnumerator coroutine = GameController.MoveCard(base.TurnTakerController, pc.CardToPlay, base.Card.UnderLocation, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine = base.AfterFlipCardImmediateResponse();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (base.CharacterCard.IsFlipped)
            {
                MoveCardDestination mcd = new MoveCardDestination(base.FindPlayAreaFromDeck(GetNativeDeck(base.CharacterCard)));

                coroutine = GameController.MoveCards(base.TurnTakerController, FindCardsWhere(new LinqCardCriteria((Card c) => c.Location == base.CharacterCard.UnderLocation)), base.FindPlayAreaFromDeck(GetNativeDeck(base.CharacterCard)), false, true, true, null, false, false, null, GetCardSource());
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
                coroutine = GameController.MoveCards(base.TurnTakerController, FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlay && c.DoKeywordsContain("nanite swarm") && !c.IsBeingDestroyed)), base.CharacterCard.UnderLocation, cardSource:GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                

                coroutine = GameController.SetHP(base.CharacterCard, FindCardsWhere((Card c) => c.Location == base.CharacterCard.UnderLocation).Count() * 10, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (CharacterCard.HitPoints <= 0)
                {
                    coroutine = DestroyThisCardResponse(null);
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
                    coroutine = DoNothing();
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

        public override bool AskIfCardIsIndestructible(Card card)
        {
            if (card == base.Card && (base.Card.IsFlipped || FindCardsWhere((Card c) => c.Location == base.CharacterCard.UnderLocation).Count() > 0))
            {
                return card.BattleZone == base.CardWithoutReplacements.BattleZone;
            }
            return false;
        }
    }
}
