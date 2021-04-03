using System;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{
    class Effect : Drawable
    {
        public FieldType? TargetCard { get; private set; }

        public PlayerType? TargetPlayer { get; private set; }

        public ViewType viewType;

        public delegate void CustomAction(Card card = null, Player player = null);

        public CustomAction action;

        private readonly Text effectText;

        private int pos;//pos on the card
        private int id;
        private Button button;

        private readonly Card card;
        Random random = new Random();
        public Effect(Card card)
        {
            this.card = card;
            pos = 1;
            id = random.Next();
            action = (card2, player) => { Console.WriteLine($"{card2}, {player} Effect activated"); };
            TargetCard = null;
            TargetPlayer = PlayerType.Player;
            //AddButton();
            effectText = HelperFunctions.NewText("Generic effect", 10, new Vector2f(10, 250), Color.Black);
        }
        public Effect(int i)
        {
            action = (card, player) => { Console.WriteLine($"{card}, {player} Effect activated"); };
            TargetCard = null;
            TargetPlayer = PlayerType.Player;
            pos = i;
            id = random.Next();
            effectText = HelperFunctions.NewText("Generic effect", 10, new Vector2f(10, 250 + 20 * pos), Color.Black);
            //AddButton();
        }
        public Effect(int i, Card card)
        {
            this.card = card;
            action = (card2, player) => { Console.WriteLine($"{card2}, {player} Effect activated"); };
            TargetCard = null;
            TargetPlayer = PlayerType.Player;
            pos = i;
            id = random.Next();
            effectText = HelperFunctions.NewText("Generic effect", 10, new Vector2f(10, 250 + 20 * pos), Color.Black);
            //AddButton();
        }

        public Effect(int i, Card card,FieldType? targetCard, PlayerType? targetPlayer, string text, CustomAction action)
        {
            this.card = card;
            this.action = action;
            this.TargetCard = targetCard;
            this.TargetPlayer = targetPlayer;
            pos = i;
            id = random.Next();
            effectText = HelperFunctions.NewText(text, 10, new Vector2f(10, 250 + 20 * pos), Color.Black);
        }

        public void ActivateEffect(Player player)
        {
            action(player: player);
        }

        public void ActivateEffect(Card card)
        {
            if (card is EffectMonster monster)
            {
                
                action(card: card);
            }
            else
            {
                action(card: card);
            }
            
        }

        public void ActivateEffect(Player player, Card card)
        {
            action(card, player);
        }

        public void AddButton()//int used to place in right spot in order
        {
            button = new Button($"{pos}", 8, 
                new Vector2f(card.boundingBox.GetGlobalBounds().Left +40, card.boundingBox.GetGlobalBounds().Top+ 50 + 30 * pos),
                new CircleShape(20) { Scale = new Vector2f(1, .5f), OutlineThickness =3, FillColor = Color.Transparent, OutlineColor = Color.Black }, SetSelectedEffect) { IsUsable = false };
        }

        public void ActivateButton()
        {


            button.IsUsable = true;

        }

        public void DeactivateButtons()
        {

            button.IsUsable = false;

        }

        public void DoAction()
        {
            button.DoAction();
        }

        public void SetSelectedEffect()
        {
            if (card is EffectMonster monster)
            {
                Console.WriteLine(monster.CanUseEffect(this));
                if (monster.CanUseEffect(this))
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
                    if (button.IsUsable)
                    {

                        target.Draw(button, states);
                    }
                }
                    

            }
           

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
        public static Effect HealAllyCard(int amount, Card ownerCard)
        {
            var effect = new Effect(1, ownerCard, FieldType.Monster, PlayerType.Player, $"Heal an Ally Card {amount} HP", (card, player) => { ((MonsterCard)card).Hp += amount; });
            

            return effect;
        }

        public static Effect HealPlayer(int amount, Card ownerCard)
        {
            var effect = new Effect(1, ownerCard, null, PlayerType.Player, $"Heal your Self {amount} HP", (card, player) => { player.Health += amount; });
            

            return effect;
        }


    }
}
