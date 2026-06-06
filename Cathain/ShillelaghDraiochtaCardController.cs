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
    public class ShillelaghDraiochtaCardController : CardController
    {
        public ShillelaghDraiochtaCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{Cathain} deals 1 target 1 melee damage.
            //Then he deals that target 1 cold damage if you played a Water card this turn,
            //1 fire damage if you played a Fire card this turn,
            //1 lightning damage if you played an Air card this turn,
            //and 1 projectile damage if you played an Earth card this turn."
            DamageSource ds = new DamageSource(GameController, CharacterCard);
            List<SelectCardDecision> selected = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, ds, 1, DamageType.Melee, 1, false, 1, storedResultsDecisions: selected, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (selected.FirstOrDefault() != null && selected.FirstOrDefault().SelectedCard != null) {

                Card selectedTarget = selected.FirstOrDefault().SelectedCard;

                bool air = false;
                bool earth = false;
                bool fire = false;
                bool water = false;

                List<PlayCardJournalEntry> plays = Journal.PlayCardEntriesThisTurn().ToList();
                foreach (PlayCardJournalEntry play in plays)
                {
                    if (play.CardPlayed.Owner == TurnTaker && GameController.DoesCardContainKeyword(play.CardPlayed, "air"))
                    {
                        air = true;
                    }
                    if (play.CardPlayed.Owner == TurnTaker && GameController.DoesCardContainKeyword(play.CardPlayed, "earth"))
                    {
                        earth = true;
                    }
                    if (play.CardPlayed.Owner == TurnTaker && GameController.DoesCardContainKeyword(play.CardPlayed, "fire"))
                    {
                        fire = true;
                    }
                    if (play.CardPlayed.Owner == TurnTaker && GameController.DoesCardContainKeyword(play.CardPlayed, "water"))
                    {
                        water = true;
                    }
                }



                //Then he deals that target 1 cold damage if you played a Water card this turn,
                if (water)
                {
                    coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c == selectedTarget, 1, DamageType.Cold, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                //1 fire damage if you played a Fire card this turn,
                if (fire)
                {
                    coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c == selectedTarget, 1, DamageType.Fire, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                //1 lightning damage if you played an Air card this turn,
                if (air)
                {
                    coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c == selectedTarget, 1, DamageType.Lightning, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                //and 1 projectile damage if you played an Earth card this turn."
                if (earth)
                {
                    coroutine = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c == selectedTarget, 1, DamageType.Projectile, cardSource: GetCardSource());
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
