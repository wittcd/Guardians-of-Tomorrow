using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
    class ChaoticNatureCardController : CardController
    {
        public ChaoticNatureCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            LinqCardCriteria isOng = new LinqCardCriteria((Card c) => c.IsOngoing, "ongoing");
            IEnumerator coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, true, false, isOng, 1, null, true, false, RevealedCardDisplay.None, false, false, null, false, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = DealDamage((Card c) => c == base.CharacterCard, (Card c) => c.IsHero, (Card c) => H - 2, StringToDamageType(((EidolonCharacterCardController)base.CharacterCardController).getMostRecentString()));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
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
