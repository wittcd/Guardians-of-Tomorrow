using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace GuardiansOfTomorrow.TheLastMonsterHunter
{
    public abstract class TLMHWeaponCardController : CardController
    {
        public TLMHWeaponCardController(Card card, TurnTakerController turnTakerController)
           : base(card, turnTakerController)
        {
        }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition definition)
        {
            IEnumerator enumerator = null;
            if (definition.Name == "attack")
            {
                enumerator = ActivateAttack();
            }
            if (enumerator != null)
            {
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(enumerator);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(enumerator);
                }
            }
        }

        public abstract IEnumerator ActivateAttack();
    }
}
