using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardiansOfTomorrow.Arsenal
{
    public class CombatTrainingCardController : CardController
    {
        public CombatTrainingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = GameController.DrawCards(TurnTakerController.ToHero(), 2, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            DamageType[] types = { DamageType.Melee, DamageType.Projectile };
            List<SelectDamageTypeDecision> selectedType = new List<SelectDamageTypeDecision>();
            coroutine = GameController.SelectDamageType(DecisionMaker, selectedType, types, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 3, selectedType.FirstOrDefault().SelectedDamageType.Value, 1, false, 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
