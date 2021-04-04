using System;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompCardGame.Source.Objects;
using CompCardGame.Source.Core;
using CompCardGame.Source.Field;

namespace CompCardGame.Source.Objects
{
    class Effect : Drawable
    {
        public FieldType? TargetCard { get; private set; }

        public PlayerType? TargetPlayer { get; private set; }

        public ViewType viewType;

        public delegate void CustomAction(Card card = null, Player player = null);

        public CustomAction action;

        private readonly Text effectText;

        public int effectCost;

        //public bool hasCost;
        private int pos;//pos on the card
        private int id;
        private int useAmount = 1;
        private int maxUseAmount = 1;
        private bool singleUse = false;
        private Button button;

        private readonly Card card;
        Random random = new Random();
        public Effect(Card card)
        {
            this.card = card;
            //if (card is SpellCard)
            //{
            //    hasCost = false;
            //}
            //else
            //{
            //    hasCost = true;
            //}
            pos = 1;
            id = random.Next();
            action = (card2, player) => { Console.WriteLine($"{card2}, {player} Effect activated"); };
            TargetCard = null;
            TargetPlayer = PlayerType.Player;
            //AddButton();
            effectText = HelperFunctions.NewText("Generic effect", 10, new Vector2f(10, 230), Color.Black);
        }
        public Effect(int i)
        {
            action = (card, player) => { Console.WriteLine($"{card}, {player} Effect activated"); };
            TargetCard = null;
            TargetPlayer = PlayerType.Player;
            pos = i;
            id = random.Next();
            effectText = HelperFunctions.NewText("Generic effect", 10, new Vector2f(10, 230 + 20 * pos), Color.Black);
            //AddButton();
        }
        public Effect(int i, Card card)
        {
            this.card = card;
            //if (card is SpellCard)
            //{
            //    hasCost = false;
            //}
            //else
            //{
            //    hasCost = true;
            //}
            action = (card2, player) => { Console.WriteLine($"{card2}, {player} Effect activated"); };
            TargetCard = null;
            TargetPlayer = PlayerType.Player;
            pos = i;
            id = random.Next();
            effectText = HelperFunctions.NewText("Generic effect", 10, new Vector2f(10, 230 + 20 * pos), Color.Black);
            //AddButton();
        }

        public Effect(int pos, Card card, FieldType? targetCard, PlayerType? targetPlayer, string text, CustomAction action, int effectCost = 1)
        {
            this.card = card;
            //if (card is SpellCard)
            //{
            //    hasCost = false;
            //}
            //else
            //{
            //    hasCost = true;
            //}
            this.action = action;
            this.TargetCard = targetCard;
            this.TargetPlayer = targetPlayer;
            this.pos = pos;
            id = random.Next();
            this.effectCost = effectCost;
            effectText = HelperFunctions.NewText(text, 10, new Vector2f(10, 230 + 20 * pos), Color.Black);
        }

        public void ActivateEffect(Player player)
        {
            action(player: player);
            useAmount -= 1;
        }

        public void ActivateEffect(Card card)
        {
            if (card is EffectMonster monster)
            {

                action(card: card);
                useAmount -= 1;
            }
            else
            {
                action(card: card);
                useAmount -= 1;
            }

        }

        public void ActivateEffect(Player player, Card card)
        {
            action(card, player);
        }

        public void AddButton()//int used to place in right spot in order
        {
            button = new Button($"{pos}", 8,
                new Vector2f(card.boundingBox.GetGlobalBounds().Left + 40, card.boundingBox.GetGlobalBounds().Top + 50 + 30 * pos),
                new CircleShape(20) { Scale = new Vector2f(1, .5f), OutlineThickness = 3, FillColor = Color.Transparent, OutlineColor = Color.Black }, SetSelectedEffect)
            { IsUsable = false };
        }

        public void ActivateButton()
        {


            button.IsUsable = true;

        }

        public void DeactivateButtons()
        {

            button.IsUsable = false;

        }

        public void ResetUseAmount()
        {
            if (!singleUse)
            {
                useAmount = maxUseAmount;
            }

        }

        public void DoAction()
        {

            button.EnableActionUse();
            button.DoAction();
        }

        public void SetSelectedEffect()
        {
            if (card is EffectMonster monster)
            {
                //Console.WriteLine(monster.CanUseEffect(this));
                if (CanUseEffect())
                {
                    Match.selectedEffect = this;
                    Match.AlertText.DisplayedString = (this.TargetCard == null) ? $"Please select {this.TargetPlayer}." : $"Please select {this.TargetCard} on {this.TargetPlayer} field.";
                }
            }
            else
            {
                Match.selectedEffect = this;
                Match.AlertText.DisplayedString = (this.TargetCard == null) ? $"Please select {this.TargetPlayer}." : $"Please select {this.TargetCard} on {this.TargetPlayer} field.";
            }
        }

        //public void RemoveButton()
        //{
        //    foreach (var button in buttons)
        //    {
        //        button.IsUsable = false;
        //    }
        //}

        public bool CanUseEffect()
        {
            if (singleUse)
            {

                return ((EffectMonster)card).Mana - effectCost >= 0 && useAmount > 0;

            }
            else if (useAmount > 0 && !singleUse)
            {
                return ((EffectMonster)card).Mana - effectCost >= 0;
            }
            else
            {
                return false;
            }
            //return ((EffectMonster)card).Mana - effectCost >= 0; 
        }

        public void Draw(RenderTarget target, RenderStates states)
        {


            if (viewType == ViewType.SideView)
            {
                target.Draw(effectText, states);
            }
            else
            {
                target.Draw(effectText, states);
                if (button != null)
                {
                    if (button.IsUsable && useAmount > 0)
                    {

                        target.Draw(button, states);
                    }
                }


            }


        }

        public void DrainMana()
        {
            ((EffectMonster)card).Mana -= effectCost;
        }

        public Boolean CheckClick(Vector2f mouse)
        {
            if (button.Contains(mouse))
            {

                return true;
            }
            return false;

        }

        public bool Equals(Effect effect)
        {
            return id == effect.id && card.Equals(effect.card);

        }
        public static Effect HealAllyCard(int amount, Card ownerCard, int pos = 1, int cost = 1, bool singleUse = false, int useAmount = 1)
        {
            var effect = new Effect(pos, ownerCard, FieldType.Monster, PlayerType.Player, $"Heal an Ally Card {amount} HP", (card, player) => { ((MonsterCard)card).Hp += amount; }, cost)
            {
                singleUse = singleUse,
                useAmount = useAmount
            };
            if (ownerCard is EffectMonster)
            {
                if (singleUse)
                {
                    if (useAmount == 0)
                    {
                        effect.effectText.DisplayedString = $"{cost} Mana: Heal an Ally Card {amount} HP.\n Single Use";

                    }
                    else
                    {
                        effect.effectText.DisplayedString = $"{cost} Mana: Heal an Ally Card {amount} HP.\n {useAmount} Uses";

                    }
                }
                else
                {
                    effect.effectText.DisplayedString = $"{cost} Mana: Heal an Ally Card {amount} HP";
                }
            }

            return effect;
        }

        public static Effect HealPlayer(int amount, Card ownerCard, int pos = 1, int cost = 1, bool singleUse = false, int useAmount = 1)
        {
            var effect = new Effect(pos, ownerCard, null, PlayerType.Player, $"Heal your Self {amount} HP", (card, player) => { player.Health += amount; }, cost)
            {
                singleUse = singleUse,
                useAmount = useAmount
            };
            if (ownerCard is EffectMonster)
            {
                if (singleUse)
                {
                    if (useAmount == 1)
                    {
                        effect.effectText.DisplayedString = $"{cost} Mana: Heal your Self {amount} HP.\n Single Use";

                    }
                    else
                    {
                        effect.effectText.DisplayedString = $"{cost} Mana: Heal your Self {amount} HP.\n {useAmount} Uses";

                    }
                }
                else
                {
                    effect.effectText.DisplayedString = $"{cost} Mana: Heal your Self {amount} HP";

                }
            }

            return effect;
        }

        public static Effect OverloadCardMana(int amount, Card ownerCard, int pos = 1, int cost = 1, bool singleUse = false, int useAmount = 1)
        {

            var effect = new Effect(pos, ownerCard, FieldType.Monster, PlayerType.Player, $"Give Monster {amount} Mana", (card, player) => { ((MonsterCard)card).Mana += amount; }, cost)
            {
                singleUse = singleUse,
                useAmount = useAmount
            };
            if (ownerCard is EffectMonster)
            {
                if (singleUse)
                {
                    if (useAmount == 1)
                    {
                        effect.effectText.DisplayedString = $"{cost} Mana: Give Monster { amount} Mana. \nSingle Use";
                    }
                    else
                    {
                        effect.effectText.DisplayedString = $"{cost} Mana: Give Monster { amount} Mana. \n{useAmount} Uses";
                    }


                }
                else
                {
                    effect.effectText.DisplayedString = $"{cost} Mana: Give Monster { amount} Man";

                }

            }

            return effect;
        }



    }
}
