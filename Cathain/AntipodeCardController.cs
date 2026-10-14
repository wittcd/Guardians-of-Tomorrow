using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Cathain
{
    public class AntipodeCardController : CardController
    {
        public AntipodeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Cathain} deals one target 3 cold or fire damage, then {Cathain} or one of your Loci deals itself 6 irreducible infernal damage.",
            List<SelectDamageTypeDecision> choice = new List<SelectDamageTypeDecision>();
            IEnumerator coroutine = GameController.SelectDamageType(DecisionMaker, choice, new DamageType[] { DamageType.Cold, DamageType.Fire }, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, choice.FirstOrDefault().SelectedDamageType.Value, 1, false, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{Cathain} or one of your Loci deals itself 6 irreducible infernal damage.
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 6, DamageType.Infernal, 1, false, 1, additionalCriteria: (Card c) => c == CharacterCard, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            
            AddTrigger<DealDamageAction>((DealDamageAction dd) => IsHeroCharacterCard(dd.DamageSource.Card), ChangeToColdOrFire, TriggerType.ChangeDamageType, TriggerTiming.Before);
            AddTrigger<DealDamageAction>((DealDamageAction dd) => !IsHero(dd.Target) && (dd.DamageType == DamageType.Cold || dd.DamageType == DamageType.Fire), IncreaseNextOtherType, TriggerType.IncreaseDamage, TriggerTiming.After);
            //Change all damage dealt by heroes to cold or fire damage. Whenever a non-hero target takes cold or fire damage, increase the next damage it takes of the other type by 1 and make it irreducible.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pc) => DestroyThisCardResponse(null), TriggerType.DestroySelf, (PhaseChangeAction pc) => !DidEnterThisTurn(pc));
        }

        private DamageType OtherDamageType(DamageType dt)
        {
            if (dt == DamageType.Cold)
            {
                return DamageType.Fire;
            }
            return DamageType.Cold;
        }

        private IEnumerator ChangeToColdOrFire(DealDamageAction dd) { 
            List<SelectDamageTypeDecision> chosenDT = new List<SelectDamageTypeDecision>();
            DamageType[] options = {DamageType.Cold, DamageType.Fire};
            IEnumerator coroutine = GameController.SelectDamageType(DecisionMaker, chosenDT, options, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.ChangeDamageType(dd, chosenDT.FirstOrDefault().SelectedDamageType.Value, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator IncreaseNextOtherType(DealDamageAction dd) {
            DamageType otherType = OtherDamageType(dd.DamageType);
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.DamageTypeCriteria.AddType(otherType);
            idse.TargetCriteria.IsSpecificCard = dd.Target;
            idse.NumberOfUses = 1;

            MakeDamageIrreducibleStatusEffect mdise = new MakeDamageIrreducibleStatusEffect();
            mdise.DamageTypeCriteria.AddType(otherType);
            mdise.TargetCriteria.IsSpecificCard = dd.Target;
            mdise.NumberOfUses = 1;

            IEnumerator coroutine = GameController.AddStatusEffectEx(idse, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.AddStatusEffectEx(mdise, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private bool DidEnterThisTurn(PhaseChangeAction pc)
        {
            List<CardEntersPlayJournalEntry> entries = Journal.CardEntersPlayEntriesThisTurn().ToList();
            foreach (CardEntersPlayJournalEntry entry in entries)
            {
                if (entry.Card == Card)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
