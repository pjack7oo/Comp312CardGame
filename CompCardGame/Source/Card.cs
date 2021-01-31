using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{
    class Card: Transformable, Drawable
    {

        Font font = new Font("Media/ARIALBD.TTF");
        Drawable[] shapes = new Drawable[3];
        Text cardName;
        Text cardDescription;
        int attack;
        int defense;
        private int width;
        private int height;
        
        public Card()
        {
            
            shapes[0] = new RectangleShape(new SFML.System.Vector2f { X = 200f, Y = 320f }) { FillColor = Color.Cyan};
            shapes[1] = new RectangleShape(new SFML.System.Vector2f { X = 180f, Y = 20f }) { OutlineColor = Color.Black,OutlineThickness = 1f, Position = Position + new SFML.System.Vector2f { X = 10f, Y = 10f } };//TODO make all of this relative to width and height
            shapes[2] = new RectangleShape(new SFML.System.Vector2f { X = 180f, Y = 180f }) { OutlineColor = Color.Black, OutlineThickness = 1f, Position = Position + new SFML.System.Vector2f { X = 10f, Y = 50f } };
            cardName = new Text();
            cardName.DisplayedString = "Card Name";
            cardName.Font = font;
            cardName.FillColor = Color.Black;
            cardName.CharacterSize = 15;
            cardName.Position = Position + new SFML.System.Vector2f { X = 10f, Y = 10f };
            cardDescription = new Text();
            cardDescription.Font = font;
            cardDescription.CharacterSize = 10;
            cardDescription.DisplayedString = "Description";
            cardDescription.FillColor = Color.Black;
            cardDescription.Position = Position + new SFML.System.Vector2f { X = 10f, Y = 235f };
            attack = 100;
            defense = 100;
            
        }

        public Card(String name, String discription, int attack, int defense)
        {
            shapes[0] = new RectangleShape(new SFML.System.Vector2f { X = 200f, Y = 320f }) { FillColor = Color.Cyan };
            shapes[1] = new RectangleShape(new SFML.System.Vector2f { X = 180f, Y = 20f }) { OutlineColor = Color.Black, OutlineThickness = 1f, Position = Position + new SFML.System.Vector2f { X = 10f, Y = 10f } };//TODO make all of this relative to width and height
            shapes[2] = new RectangleShape(new SFML.System.Vector2f { X = 180f, Y = 180f }) { OutlineColor = Color.Black, OutlineThickness = 1f, Position = Position + new SFML.System.Vector2f { X = 10f, Y = 50f } };
            cardName = new Text();
            cardName.DisplayedString = name;
            cardName.Font = font;
            cardName.FillColor = Color.Black;
            cardName.CharacterSize = 15;
            cardName.Position = Position + new SFML.System.Vector2f { X = 10f, Y = 10f };
            cardDescription = new Text();
            cardDescription.Font = font;
            cardDescription.CharacterSize = 10;
            cardDescription.DisplayedString = discription;
            cardDescription.FillColor = Color.Black;
            cardDescription.Position = Position + new SFML.System.Vector2f { X = 10f, Y = 235f };
            this.attack = attack;
            this.defense = defense;
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform = Transform;
            foreach(var shape in shapes)
            {
                target.Draw(shape, states);
            }
            //states.Transform.Combine(text.Transform);
            //states.Transform = Transform;
            target.Draw(cardName, states);
            target.Draw(cardDescription, states);
            target.Draw(new Text("Attack", font,15) { Position = new SFML.System.Vector2f { X = 10f, Y = 300f } }, states);
            target.Draw(new Text(attack.ToString(), font, 15) { Position = new SFML.System.Vector2f { X = 70f, Y = 300f } }, states);
            target.Draw(new Text("Defense", font, 15) { Position = new SFML.System.Vector2f { X = 100f, Y = 300f } }, states);
            target.Draw(new Text(defense.ToString(), font, 15) { Position = new SFML.System.Vector2f { X = 170f, Y = 300f } }, states);
        }
    }
}
