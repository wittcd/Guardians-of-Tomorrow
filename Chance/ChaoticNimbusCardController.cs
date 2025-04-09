using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace GuardiansOfTomorrow.Chance
{
    class ChaoticNimbusCardController : ChanceCardController
    {
        public ChaoticNimbusCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DamageType> Unused = new List<DamageType>();
            Unused.Add(DamageType.Cold);
            Unused.Add(DamageType.Energy);
            Unused.Add(DamageType.Fire);
            Unused.Add(DamageType.Infernal);
            Unused.Add(DamageType.Lightning);
            Unused.Add(DamageType.Melee);
            Unused.Add(DamageType.Projectile);
            Unused.Add(DamageType.Psychic);
            Unused.Add(DamageType.Radiant);
            Unused.Add(DamageType.Sonic);
            Unused.Add(DamageType.Toxic);

            bool flippedTails = false;

            while (!flippedTails)
            {
                List<SelectDamageTypeDecision> resultType = new List<SelectDamageTypeDecision>();
                IEnumerator coroutine = GameController.SelectDamageType(DecisionMaker, resultType, Unused, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }


                DamageType selected = GetSelectedDamageType(resultType).Value;
                Unused.Remove(selected);

                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, base.CharacterCard), 1, selected, 1, false, 0, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                List<int> coin = new List<int>();
                coroutine = FlipCoin(coin, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (coin.Count() <= 0 || coin.FirstOrDefault() != 1 || Unused.Count() <= 0)
                {
                    flippedTails = true;
                }
            }
        }
    }
}
