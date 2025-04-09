using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Price
{
    class BloodSacrificeCardController : CardController
    {
        public BloodSacrificeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        
        }

        public override IEnumerator Play()
        {
            List<DealDamageAction> damages = new List<DealDamageAction>();
            damages.Add(new DealDamageAction(GameController, new DamageSource(GameController, CharacterCard), null, 2, DamageType.Melee));
            damages.Add(new DealDamageAction(GameController, new DamageSource(GameController, CharacterCard), null, 2, DamageType.Infernal));
            List<DealDamageAction> results = new List<DealDamageAction>();
            IEnumerator coroutine = SelectTargetAndDealMultipleInstancesOfDamageEx(damages, null, results, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            bool damagedPrice = false;
            foreach (DealDamageAction d in results)
            {
                if (d.DidDealDamage && !d.DidDestroyTarget && d.Target == CharacterCard)
                {
                    damagedPrice = true;
                }
            }

            if (damagedPrice)
            {
                List<MoveCardDestination> t = new List<MoveCardDestination>();
                t.Add(new MoveCardDestination(TurnTaker.Trash));
                coroutine = GameController.SelectCardFromLocationAndMoveIt(DecisionMaker, TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("pact"), "Pact"), t, isDiscardIfMovingtoTrash: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = ShuffleDeck(DecisionMaker, TurnTaker.Deck);
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
