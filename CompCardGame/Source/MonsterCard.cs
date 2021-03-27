using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{

    
    class MonsterCard: Card
    {


        private int attack;
        private int defense;
        private int mana;
        private int maxMana;
        public int attackManaCost;


        Text cardAttackText;
        Text cardDefenseText;
        Text cardManaText;
        Text cardMaxManaText;

        public int Attack { get { return attack; } set { attack = value; cardAttackText.DisplayedString = "Attack: " + value.ToString(); } }
        public int Defense { get { return defense; } set { defense = value; cardDefenseText.DisplayedString = "Defense: " + value.ToString(); } }
        public int Mana { get { return mana; } set { mana = value; cardManaText.DisplayedString = "ManaPool: " + value.ToString(); } }

        public int MaxMana { get { return maxMana; } set { maxMana = value; cardMaxManaText.DisplayedString = "MaxMana: " + value.ToString(); } }//future it will be shown by top filled mana pool icon
        public MonsterCard(): base()
        {
            this.SetColors(new Color(210, 180, 140), Color.Black);
            cardAttackText = HelperFunctions.NewText("Attack: ", 15, new Vector2f { X = 5f, Y = height - 20f }, Color.Black);
            cardDefenseText = HelperFunctions.NewText("Defense: ", 15, new Vector2f { X = 100f, Y = height - 20f }, Color.Black);
            cardManaText = HelperFunctions.NewText("ManaPool: ", 15, new Vector2f { X = 5f, Y = height - 35f }, Color.Black);
            cardMaxManaText = HelperFunctions.NewText("MaxMana: ", 15, new Vector2f { X = 100f, Y = height - 35f }, Color.Black);


            Attack = 100;
            Defense = 100;
            Mana = 1;
            MaxMana = 1;
            attackManaCost = 1;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
            if (viewType == ViewType.SideView)
            {
                if (State == CardState.Front)
                {
                    states.Transform.Translate(new Vector2f(Game.SideViewWidth - Card.width * 1.4f, Game.ScreenHeight - Card.height * 1.4f - 50));
                    //drawing each of the shapes
                    //foreach (var shape in shapes)
                    //{
                    //    target.Draw(shape, states);
                    //}
                    //drawing the text
                    //target.Draw(cardName, states);
                    //target.Draw(cardDescription, states);
                    target.Draw(cardAttackText, states);
                    target.Draw(cardDefenseText, states);
                    target.Draw(cardManaText, states);
                    target.Draw(cardMaxManaText, states);

                    
                    
                }
            }
            else
            {


                //applying object transform to the states transform for drawing uniformly
                states.Transform = Transform;
                //drawing based on the state
                if (State == CardState.Front)
                {
                    
                    
                    target.Draw(cardAttackText, states);
                    target.Draw(cardDefenseText, states);
                    target.Draw(cardManaText, states);
                    target.Draw(cardMaxManaText, states);

                    
                    
                }
               
            }
        }

        
    }
}
