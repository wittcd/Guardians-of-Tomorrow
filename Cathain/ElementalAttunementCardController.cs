using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Cathain
{
    public class ElementalAttunementCardController : CardController
    {
        public ElementalAttunementCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Whenever you play a non-locus card that shares a keyword with a Locus in play, you may draw a card or {Cathain} gains 1 HP.
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cep) => !GameController.DoesCardContainKeyword(cep.CardEnteringPlay, "locus") && GetLocusOverlap(cep.CardEnteringPlay), (CardEntersPlayAction cep) => DrawOrHeal(), new TriggerType[] { TriggerType.DrawCard, TriggerType.GainHP }, TriggerTiming.After);
        }

        private IEnumerator DrawOrHeal()
        {
            List<Function> options = new List<Function>();
            options.Add(new Function(DecisionMaker, "draw a card", SelectionType.DrawCard, () => GameController.DrawCard(TurnTaker.ToHero(), optional: true, cardSource: GetCardSource())));
            options.Add(new Function(DecisionMaker, "{Cathain} gains 1 HP", SelectionType.GainHP, () => GameController.GainHPEx(CharacterCard, 1, cardSource: GetCardSource())));
            SelectFunctionDecision sfd = new SelectFunctionDecision(GameController, DecisionMaker, options, true, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(sfd);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private bool getKeywordOverlap(IEnumerable<String> keywords, Card c)
        {
            List<String> k = c.GetKeywords().ToList();
            foreach (String s in k)
            {
                if (keywords.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }

        private bool GetLocusOverlap(Card card)
        {
            List<Card> loci = FindCardsWhere((Card c) => GameController.DoesCardContainKeyword(c, "locus") && c.Owner == TurnTaker && c.IsInPlayAndNotUnderCard).ToList();
            if (loci.Count > 0)
            {
                List<String> locusKeywords = new List<String>();
                foreach (Card l in loci)
                {
                    locusKeywords.AddRange(l.GetKeywords());
                }

                return getKeywordOverlap(locusKeywords, card);
            }
            return false;
        }
    }
}
