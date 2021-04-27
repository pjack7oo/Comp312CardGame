using System;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystal_Wars.Source.Objects;
using Crystal_Wars.Source.Core;
using Crystal_Wars.Source.Field;
using Newtonsoft.Json;
namespace Crystal_Wars.Source.Objects
{
    [JsonObject(MemberSerialization.OptOut)]
    class Effect : Drawable
    {
        public FieldType? TargetCard { get; private set; }

        public PlayerType? TargetPlayer { get; private set; }

        public ViewType viewType;
        
        public delegate void CustomAction(Card card = null, Player player = null);
        public string actionName;
        [NonSerialized]
        public CustomAction action;
        [NonSerialized]
        private readonly Text effectText;

        public string effectString;
        public int effectCost;

        public int effectAmount;
        //public bool hasCost;
        public int pos;//pos on the card
        public int id;
        private int useAmount = 1;
        private int maxUseAmount = 1;
        private bool singleUse = false;
        [NonSerialized]
        private Button button;

        [NonSerialized]
        private  Card card;
        [NonSerialized]
        Random random = new Random();
        [JsonConstructor]
        public Effect(FieldType? TargetCard, PlayerType? TargetPlayer, ViewType viewType, int effectCost, int pos, int id, int useAmount, int maxUseAmount, bool singleUse,string actionName, string effectString, int effectAmount)
        {
            this.TargetCard = TargetCard;
            this.TargetPlayer = TargetPlayer;
            this.viewType = viewType;
            
            this.effectCost = effectCost;
            this.pos = pos;
            this.id = id;
            this.useAmount = useAmount;
            this.maxUseAmount = maxUseAmount;
            this.singleUse = singleUse;
            this.actionName = actionName;
            this.effectString = effectString;
            this.effectAmount = effectAmount;
            this.action = GetEffect(actionName, effectAmount);
            effectText = HelperFunctions.NewText(this.effectString, 10, new Vector2f(10, 230 + 20 * pos), Color.Black);

        }
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
            effectString = "Generic effect";
            effectText = HelperFunctions.NewText("Generic effect", 10, new Vector2f(10, 230), Color.Black);
        }
        public Effect(int i)
        {
            action = (card, player) => { Console.WriteLine($"{card}, {player} Effect activated"); };
            TargetCard = null;
            TargetPlayer = PlayerType.Player;
            pos = i;
            id = random.Next();
            effectString = "Generic effect";
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
            effectString = "Generic effect";
            effectText = HelperFunctions.NewText("Generic effect", 10, new Vector2f(10, 230 + 20 * pos), Color.Black);
            //AddButton();
        }
        //todo
        //private Effect GetEffect(string effect)
        //{
        //    if (effect == "HealAllyCard")
        //    {
        //        return 
        //    }
        //}

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
            effectString = text;
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
                new Vector2f(card.boundingBox.GetGlobalBounds().Left + 40, card.boundingBox.GetGlobalBounds().Top + 50 + 30 * pos),Color.Black,
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
            if (button != null)
            {
                if (button.Contains(mouse))
                {

                    return true;
                }
            }
            
            return false;

        }

        public bool Equals(Effect effect)
        {
            return id == effect.id && card.Equals(effect.card);

        }
        public void SetCard(Card card)
        {
            this.card = card;
        }

        public static CustomAction GetEffect(string action, int effectAmount)
        {
            switch(action)
            {
                case ("HealAllyCard"):
                    return HealAllyCard(effectAmount);
                case ("HealPlayer"):
                    return HealPlayer(effectAmount);
                case ("OverloadCardMana"):
                    return OverloadCardMana(effectAmount);
                default:
                    return null;
                    break;
            }
        }

        public static CustomAction HealAllyCard(int amount)
        {
            return (card, player) => { ((MonsterCard)card).Hp += amount; };
        }


        public static CustomAction OverloadCardMana(int amount)
        {
            return (card, player) => { ((MonsterCard)card).Mana += amount; };
        }


        public static CustomAction HealPlayer(int amount)
        {
            return (card, player) => { player.Health += amount; };
        }

        public static Effect HealAllyCard(int amount, Card ownerCard, int pos = 1, int cost = 1, bool singleUse = false, int useAmount = 1)
        {
            var effect = new Effect(pos, ownerCard, FieldType.Monster, PlayerType.Player, $"Heal an Ally Card {amount} HP", (card, player) => { ((MonsterCard)card).Hp += amount; }, cost)
            {
                singleUse = singleUse,
                useAmount = useAmount,
                actionName = "HealAllyCard"
            };
            if (ownerCard is EffectMonster)
            {
                if (singleUse)
                {
                    if (useAmount == 0)
                    {
                        effect.effectText.DisplayedString = $"{cost} Mana: Heal an Ally Card {amount} HP.\n Single Use";
                        effect.effectString = $"{cost} Mana: Heal an Ally Card {amount} HP.\n Single Use";
                    }
                    else
                    {
                        effect.effectText.DisplayedString = $"{cost} Mana: Heal an Ally Card {amount} HP.\n {useAmount} Uses";
                        effect.effectString = $"{cost} Mana: Heal an Ally Card {amount} HP.\n {useAmount} Uses";
                    }
                }
                else
                {
                    effect.effectText.DisplayedString = $"{cost} Mana: Heal an Ally Card {amount} HP";
                    effect.effectString = $"{cost} Mana: Heal an Ally Card {amount} HP";
                }
            }

            return effect;
        }

        public static Effect HealPlayer(int amount, Card ownerCard, int pos = 1, int cost = 1, bool singleUse = false, int useAmount = 1)
        {
            var effect = new Effect(pos, ownerCard, null, PlayerType.Player, $"Heal your Self {amount} HP", (card, player) => { player.Health += amount; }, cost)
            {
                singleUse = singleUse,
                useAmount = useAmount,
                actionName = "HealPlayer"
            };
            if (ownerCard is EffectMonster)
            {
                if (singleUse)
                {
                    if (useAmount == 1)
                    {
                        effect.effectText.DisplayedString = $"{cost} Mana: Heal your Self {amount} HP.\n Single Use";
                        effect.effectString = $"{cost} Mana: Heal your Self {amount} HP.\n Single Use";
                    }
                    else
                    {
                        effect.effectText.DisplayedString = $"{cost} Mana: Heal your Self {amount} HP.\n {useAmount} Uses";
                        effect.effectString = $"{cost} Mana: Heal your Self {amount} HP.\n {useAmount} Uses";
                    }
                }
                else
                {
                    effect.effectText.DisplayedString = $"{cost} Mana: Heal your Self {amount} HP";
                    effect.effectString = $"{cost} Mana: Heal your Self {amount} HP";
                }
            }

            return effect;
        }

        public static Effect OverloadCardMana(int amount, Card ownerCard, int pos = 1, int cost = 1, bool singleUse = false, int useAmount = 1)
        {

            var effect = new Effect(pos, ownerCard, FieldType.Monster, PlayerType.Player, $"Give Monster {amount} Mana", (card, player) => { ((MonsterCard)card).Mana += amount; }, cost)
            {
                singleUse = singleUse,
                useAmount = useAmount,
                actionName = "OverloadCardMana"
            };
            if (ownerCard is EffectMonster)
            {
                if (singleUse)
                {
                    if (useAmount == 1)
                    {
                        effect.effectText.DisplayedString = $"{cost} Mana: Give Monster { amount} Mana. \nSingle Use";
                        effect.effectString = $"{cost} Mana: Give Monster { amount} Mana. \nSingle Use";
                    }
                    else
                    {
                        effect.effectText.DisplayedString = $"{cost} Mana: Give Monster { amount} Mana. \n{useAmount} Uses";
                        effect.effectString = $"{cost} Mana: Give Monster { amount} Mana. \n{useAmount} Uses";
                    }


                }
                else
                {
                    effect.effectText.DisplayedString = $"{cost} Mana: Give Monster { amount} Man";
                    effect.effectString = $"{cost} Mana: Give Monster { amount} Man";
                }

            }

            return effect;
        }



    }
}
