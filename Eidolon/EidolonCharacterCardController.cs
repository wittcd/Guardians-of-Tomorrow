using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.Eidolon
{
	class EidolonCharacterCardController : VillainCharacterCardController
	{
		public DamageType MostRecentDamageType;

		//private List<DamageType> Immunities = new List<DamageType>();

		/*public string GetImmuneListAsString()
        {
			string immuneList = "";
			for (int x = 0; x < Immunities.Count(); x++)
			{
				immuneList += Immunities.ElementAt(x).ToString();
				if (x < Immunities.Count() - 2)
				{
					immuneList += ", ";
				}
				else if (x == Immunities.Count() - 2)
				{
					immuneList += " and ";
				}
				else
				{
					immuneList += " ";
				}
			}
			return immuneList;
		}*/
		public string getMostRecentString()
        {
			IEnumerable<string> MRDamage = GameController.GetCardPropertyJournalEntryStringList(base.Card, MostRecent, false);
			if (MRDamage == null)
            {
				MRDamage = new List<string>();
            }
			if (MRDamage.Count() == 0)
            {
				return "energy";
            }
			else
            {
				return MRDamage.FirstOrDefault();
            }
        }
		private string getImmuneString()
        {
			IEnumerable<string> immuneDamageTypes = GameController.GetCardPropertyJournalEntryStringList(base.Card, ImmuneList, false);
			if (immuneDamageTypes == null)
            {
				immuneDamageTypes = new List<string>();
            }
			
			if (immuneDamageTypes.Count() == 1)
            {
				return "Eidolon is immune to " + immuneDamageTypes.FirstOrDefault() + " damage.";
            }
			else if (immuneDamageTypes.Count() > 0)
			{
				return "Eidolon is immune to " + GenericEnumerationExtensions.ToCommaList<string>(immuneDamageTypes, true) + " damage.";
			}
			else
            {
				return "Eidolon is not immune to any damage types.";
            }
        }

		private const string ImmuneList = "DamageImmunities";

		private const string MostRecent = "MostRecentDamageTaken";





		public EidolonCharacterCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController)
		{
			//MostRecentDamageType = DamageType.Energy;
			base.SpecialStringMaker.ShowSpecialString(() => "The last damage dealt to Eidolon was " + getMostRecentString() + ".");
			base.SpecialStringMaker.ShowSpecialString(() => getImmuneString()).Condition = () => !base.Card.IsFlipped;
			//GameController.AddCardPropertyJournalEntry(base.Card, ImmuneList, new List<string>());
		}

		public override void AddTriggers()
		{
			CannotPlayCards((TurnTakerController ttc) => ttc == base.TurnTakerController && base.CharacterCard.IsFlipped);
			AddDefeatedIfDestroyedTriggers(false);
			base.AddTriggers();
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

		public override void AddSideTriggers()
		{
			if (!base.Card.IsFlipped)
			{
				/*
				 * "If {Eidolon} has not yet been dealt damage this game, effects which reference the most recent damage type dealt to it deal energy damage.",
				 * "After {Eidolon} is dealt damage, it becomes immune to the damage type.",
				 * "At the start of the villain turn, if {Eidolon} is immune to {H} or more damage types, {Eidolon} flips.",
				 * "At the end of the villain turn, {Eidolon} deals the hero target with the higest HP {H - 1} damage of the type most recently dealt to {Eidolon}."
				 */
				if (base.IsGameAdvanced)
				{
					AddSideTrigger(AddReduceDamageTrigger((Card c) => c == base.CharacterCard, 1));
				}
				AddSideTrigger(AddImmuneToDamageTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && GameController.GetCardPropertyJournalEntryStringList(base.Card, ImmuneList, false) != null && GameController.GetCardPropertyJournalEntryStringList(base.Card, ImmuneList, false).Contains(dd.DamageType.ToString())));
				AddSideTrigger(AddTrigger((DealDamageAction dd) => dd.DidDealDamage && dd.Target == base.CharacterCard, AddNewImmunityResponse, TriggerType.Other, TriggerTiming.After));
				if (!base.IsGameChallenge)
                {
					AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, FlipThisCharacterCardResponse, TriggerType.FlipCard, (PhaseChangeAction pc) => GameController.GetCardPropertyJournalEntryStringList(base.Card, ImmuneList, false).Count() >= H, false));
				}
				else
                {
					AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, FlipThisCharacterCardResponse, TriggerType.FlipCard, (PhaseChangeAction pc) => GameController.GetCardPropertyJournalEntryStringList(base.Card, ImmuneList, false).Count() >= H + 2, false));
				}
				AddSideTrigger(AddTrigger((PlayCardAction pc) => pc.CardToPlay.DoKeywordsContain("alteration"), DealDamageBecauseAlterationResponse, TriggerType.DealDamage, TriggerTiming.Before));
				AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pc) => DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => c.IsHero, (Card c) => H - 1, StringToDamageType(getMostRecentString())), TriggerType.DealDamage));
			}
			else
			{
				/*"Eidolon may not play cards.",
				 * "At the start of the villain turn, if {Eidolon} did not flip this turn, {Eidolon} flips.",
				 * "At the end of the villain turn, for each damage type {Eidolon} is immune to, {Eidolon} deals each hero target 2 irreducible damage of that type. Then Eidolon ceases to be immune to that damage type."
				*/
				if (base.IsGameAdvanced)
				{
					AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == base.CharacterCard, 1));
				}
				AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, FlipThisCharacterCardResponse, TriggerType.FlipCard, (PhaseChangeAction pc) => !base.GameController.Game.Journal.WasCardFlippedThisTurn(base.Card)));
				AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageAndRemoveImmunitiesResponse, TriggerType.DealDamage, null, false));
			}

			base.AddSideTriggers();
		}

		private IEnumerator DealDamageBecauseAlterationResponse(PlayCardAction pc)
        {
			List<Card> played = new List<Card>();
			played.Add(pc.CardToPlay);
			IEnumerator coroutine = GameController.SendMessageAction(pc.CardToPlay.Title + " is an alteration card, so Eidolon deals damage to all hero targets.", Priority.High, GetCardSource(), played, false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = DealDamage(base.CharacterCard, (Card c) => !c.IsVillain, 1, StringToDamageType(getMostRecentString()));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		private IEnumerator AddNewImmunityResponse(DealDamageAction dd)
        {
			List<string> recent = new List<string>();
			recent.Add(dd.DamageType.ToString());
			GameController.AddCardPropertyJournalEntry(base.Card, MostRecent, recent);

			IEnumerable<string> i = GameController.GetCardPropertyJournalEntryStringList(base.Card, ImmuneList, false);
			if (i == null)
            {
				i = new List<string>();
            }
			List<string> immune = i.ToList();
			immune.Add(dd.DamageType.ToString());
			GameController.AddCardPropertyJournalEntry(base.Card, ImmuneList, immune);

			IEnumerator coroutine = GameController.SendMessageAction("Eidolon becomes immune to " + dd.DamageType.ToString() + " damage.", Priority.Medium, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		private IEnumerator DealDamageAndRemoveImmunitiesResponse(PhaseChangeAction pc) 
		{
			List<String> immunities = GameController.GetCardPropertyJournalEntryStringList(base.Card, ImmuneList, false).ToList();
			IEnumerator coroutine = GameController.SendMessageAction("Eidolon is no longer immune to any damage types.", Priority.Medium, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			foreach (string s in immunities)
            {
				coroutine = DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => c.IsHero, (Card c) => 3, StringToDamageType(s), true);
				//coroutine = DealDamage((Card c) => c == base.CharacterCard, (Card c) => c.IsHero, (Card c) => 2, d, true, false, null, null, null, false, null, null, false);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			immunities.Clear();
			GameController.AddCardPropertyJournalEntry(base.Card, ImmuneList, immunities);
			IEnumerator coroutine2 = DoNothing();
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
		}
	}
}
