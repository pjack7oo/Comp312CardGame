﻿using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{
    public enum CardState
    {
        Front,
        Back
    }
    class Card: Transformable, Drawable
    {
        //font used by card might be moved to helper class containing stuff like fonts and maybe loading in graphics
        public static Font font = new Font("Media/ARIALBD.TTF");
        //shapes used to draw the card
        Drawable[] shapes;
        //card attributes
        Text cardName;
        Text cardDescription;
        Text cardAttackText;
        Text cardDefenseText;
       
        //used to know which side to draw
        private CardState state = CardState.Back;

        private int attack;
        private int defense;
        private const float width = 200f;
        private const float height = 320f;

        public int Attack { get { return attack; } set { attack = value; cardAttackText.DisplayedString = "Attack: " + value.ToString(); } }
        public int Defense { get { return defense; } set { defense = value; cardDefenseText.DisplayedString = "Defense: " + value.ToString(); } }
        //generic constructor will be rarely used was mainly for testing
        public Card()
        {
            //creating the shapes of the card
            shapes = CardShapes(Position, Color.Cyan, Color.Black);
            
            
            //filling in the name and description
            cardName = NewText("Card Name", 15, Position + new Vector2f {X = 10f, Y = 10f }, Color.Black);
            cardDescription = NewText("Description", 10, Position + new Vector2f { X = 10f, Y = 235f }, Color.Black);
            cardAttackText = NewText("Attack: ", 15, Position + new Vector2f { X = 10f, Y = height - 20f }, Color.Black);
            cardDefenseText = NewText("Defense: ", 15, Position + new Vector2f { X = 100f, Y = height - 20f }, Color.Black);
            //attributes for dealing damage and defense yugioh does in hundreds, Hearthstone is in singles digits not sure which to use
            Attack  = 100;
            Defense = 100;
            
        }
        
        public Card(String name, String discription, int attack, int defense)
        {
            //creating the shapes of the card
            shapes = CardShapes(Position, Color.Cyan, Color.Black);

            //filling in the name and description
            cardName = NewText(name, 15, Position + new Vector2f { X = 10f, Y = 10f }, Color.Black);
            cardDescription = NewText(discription, 10, Position + new Vector2f { X = 10f, Y = 235f }, Color.Black);
            cardAttackText = NewText("Attack: ", 15, Position + new Vector2f { X = 10f, Y = height - 20f }, Color.Black);
            cardDefenseText = NewText("Defense: ", 15, Position + new Vector2f { X = 100f, Y = height - 20f }, Color.Black);


            Attack = attack;
            Defense = defense;
        }

        

        
        public void Draw(RenderTarget target, RenderStates states)
        {
            //applying object transform to the states transform for drawing uniformly
            states.Transform = Transform;
            //drawing each of the shapes 
            foreach(var shape in shapes)
            {
                target.Draw(shape, states);
            }
            //drawing the text
            target.Draw(cardName, states);
            target.Draw(cardDescription, states);
            target.Draw(cardAttackText, states);
            target.Draw(cardDefenseText, states);
            //target.Draw(new Text("Attack", font,15) { Position = new Vector2f { X = 10f, Y = height - 20f } }, states);
            //target.Draw(new Text(Attack.ToString(), font, 15) { Position = new Vector2f { X = 70f, Y = height - 20f } }, states);
            //target.Draw(new Text("Defense", font, 15) { Position = new Vector2f { X = 100f, Y = height - 20f } }, states);
            //target.Draw(new Text(Defense.ToString(), font, 15) { Position = new Vector2f { X = 170f, Y = height - 20f } }, states);
        }

        private static Drawable[] CardShapes(Vector2f position, Color fillColor, Color accentColor)
        {
            Drawable[] shapes = new Drawable[3];
            //creating the shapes of the card
            shapes[0] = new RectangleShape(new Vector2f { X = width, Y = height }) { FillColor = fillColor };
            shapes[1] = new RectangleShape(new Vector2f { X = width - 20f, Y = 20f }) { OutlineColor = accentColor, OutlineThickness = 1f, Position = position + new Vector2f { X = 10f, Y = 10f } };//TODO make all of this relative to width and height
            shapes[2] = new RectangleShape(new Vector2f { X = width - 20f, Y = width - 20f }) { OutlineColor = accentColor, OutlineThickness = 1f, Position = position + new Vector2f { X = 10f, Y = 50f } };

            return shapes;
        }

        public static Text NewText(String text, uint charSize, Vector2f position, Color fillColor)
        {
            return new Text() { DisplayedString = text, Font = font, CharacterSize = charSize, FillColor = fillColor, Position = position};
        }
        
    }
}
