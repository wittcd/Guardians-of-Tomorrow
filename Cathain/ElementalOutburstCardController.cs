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
    public class ElementalOutburstCardController : CardController
    {
        public ElementalOutburstCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //Play up to 3 one-shots that share a keyword with a Locus in play. Immediately end your turn.
            IEnumerator coroutine = GameController.SelectAndPlayCardsFromHand(DecisionMaker, 3, false, 0, new LinqCardCriteria((Card c) => GetLocusOverlap(c) && !GameController.DoesCardContainKeyword(c, "locus")), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.ImmediatelyEndTurn(TurnTakerController, GetCardSource());
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
