using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public class TechnologicallySuperiorCardController : CardController
    {
        public TechnologicallySuperiorCardController(Card card, TurnTakerController turnTakerController)
                    : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DealDamageAction> damages = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamage(CharacterCard, (Card c) => !IsVillain(c), 2, DamageType.Lightning, storedResults: damages);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(damages))
            {
                foreach (DealDamageAction damage in  damages)
                {
                    if (damage.DidDealDamage && IsHeroCharacterCard(damage.Target))
                    {
                        coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlayAndNotUnderCard && !c.IsCharacter && c.Location.HighestRecursiveLocation == damage.Target.Owner.PlayArea, "non-character"), false, cardSource: GetCardSource());
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
}
