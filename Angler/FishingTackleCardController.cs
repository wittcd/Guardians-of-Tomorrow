using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Angler
{
    public class FishingTackleCardController : CardController
    {
        public FishingTackleCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //{Angler} deals 1 target 3 melee damage. Either play a Bait card or redirect the next damage dealt by that target to Angler.

            List<SelectCardDecision> selected = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 3, DamageType.Melee, 1, false, 1, storedResultsDecisions: selected, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<Function> options = new List<Function>();
            options.Add(new Function(DecisionMaker, "Play a Bait Card", SelectionType.PlayCard, () => GameController.SelectAndPlayCardFromHand(DecisionMaker, false, null, new LinqCardCriteria((Card c) => c.DoKeywordsContain("bait"), "bait"), cardSource: GetCardSource()), FindCardsWhere((Card c) => c.DoKeywordsContain("bait") && c.Location == TurnTaker.ToHero().Hand).Count() > 0));
            
            if (DidSelectCard(selected))
            {
                Card targ = GetSelectedCard(selected);
                RedirectDamageStatusEffect rdse = new RedirectDamageStatusEffect();
                rdse.SourceCriteria.IsSpecificCard = targ;
                rdse.RedirectTarget = CharacterCard;
                rdse.NumberOfUses = 1;
                options.Add(new Function(DecisionMaker, "Redirect the next damage dealt by " + targ.Title + " to " + CharacterCard.Title, SelectionType.RedirectDamage, () => GameController.AddStatusEffectEx(rdse, true, GetCardSource())));
            }
            
            SelectFunctionDecision select = new SelectFunctionDecision(GameController, DecisionMaker, options, false, cardSource: GetCardSource());
            coroutine = GameController.SelectAndPerformFunction(select);
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
