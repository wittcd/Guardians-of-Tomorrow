using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
namespace GuardiansOfTomorrow.Ninetails
{

	public class AssertHumanityCardController : CardController
	{

		private DealDamageAction DealDamageAction { get; set; }

		public AssertHumanityCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
        {
        }



        public override IEnumerator Play()
        {
            //destroy one of your ongoings
            List<DestroyCardAction> storedDestroy = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) && c.Owner == base.TurnTaker, "ongoing"), true, storedDestroy, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //if you do, deal up to 2 targets 3 melee damage each.
            if (storedDestroy.Count() > 0 && storedDestroy.Any((DestroyCardAction dc) => dc.CardToDestroy.Card.Owner == base.TurnTaker)) 
            {
                DamageSource ds = new DamageSource(base.GameController, base.CharacterCard);
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, ds, 3, DamageType.Melee, 2, false, 0, false, false, false, null, null, null, null, null, false, null, null, false, null, GetCardSource());
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