using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class StrangePowersCardController : CardController
    {
        public StrangePowersCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroWithMostCards(false);
        }

        public override IEnumerator Play()
        {
            List<TurnTaker> mostHero = new List<TurnTaker>();
            IEnumerator coroutine = FindHeroWithMostCardsInPlay(mostHero, 1, 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (mostHero.Count() > 0)
            {
                coroutine = DealDamage(base.CharacterCard, (Card c) => c.IsCharacter && mostHero.Contains(c.Owner), (Card c) => H - 2, StringToDamageType(((EidolonCharacterCardController)base.CharacterCardController).getMostRecentString()), false, false, null, null, DestroyOwnedCardsResponse, false, null, null, false, true);
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

        private IEnumerator DestroyOwnedCardsResponse(DealDamageAction dd)
        {
            if (dd.DidDealDamage && dd.Target.IsHeroCharacterCard)
            {
                IEnumerator coroutine = GameController.SelectAndDestroyCards(FindTurnTakerController(dd.Target.Owner).ToHero(), new LinqCardCriteria((Card c) => !c.IsCharacter && c.IsInPlay && c.Owner == dd.Target.Owner, "non-character"), 2, cardSource: GetCardSource());
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

		public DamageType StringToDamageType(string s)
		{
			s = s.ToLower();
			if (s == "cold")
			{
				return DamageType.Cold;
			}
			else if (s == "energy")
			{
				return DamageType.Energy;
			}
			else if (s == "fire")
			{
				return DamageType.Fire;
			}
			else if (s == "infernal")
			{
				return DamageType.Infernal;
			}
			else if (s == "lightning")
			{
				return DamageType.Lightning;
			}
			else if (s == "melee")
			{
				return DamageType.Melee;
			}
			else if (s == "projectile")
			{
				return DamageType.Projectile;
			}
			else if (s == "psychic")
			{
				return DamageType.Psychic;
			}
			else if (s == "radiant")
			{
				return DamageType.Radiant;
			}
			else if (s == "sonic")
			{
				return DamageType.Sonic;
			}
			else if (s == "toxic")
			{
				return DamageType.Toxic;
			}
			else
			{
				return DamageType.Energy;
			}
		}
	}
}
