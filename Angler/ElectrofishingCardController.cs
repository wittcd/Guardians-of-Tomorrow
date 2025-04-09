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
    public class ElectrofishingCardController : CardController
    {
        public ElectrofishingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DealDamageAction> stored = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => !IsHero(c), 1, DamageType.Lightning, false, storedResults: stored, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            foreach (DealDamageAction action in stored) 
            {
                if (action.DidDealDamage)
                {
                    Card targ = action.Target;
                    ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(2);
                    rdse.UntilStartOfNextTurn(TurnTaker);
                    rdse.SourceCriteria.IsSpecificCard = targ;

                    coroutine = GameController.AddStatusEffectEx(rdse, true, GetCardSource());
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
