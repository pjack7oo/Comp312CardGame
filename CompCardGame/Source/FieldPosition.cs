using System;
using SFML.System;
using SFML.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{
    
    class FieldPosition: Drawable
    {
        private Card card;
        public Card Card { get { return card; } set { card = value; card.Position = topLeftCorner; } }
        public int position; //temporary way of store card position it might be changed
        public Boolean HasCard { get { return (this.card != null) ? true : false; } }
        //important for drawing things to the right spot
        private Vector2f topLeftCorner;
        private RectangleShape outline;


        public FieldPosition(int position)
        {
            this.position = position;
            //setting the corner for top field and bottom field
            if (position > 5)
            {
                topLeftCorner = new Vector2f((position-6)*(Card.width+20)+410,160);
                //target.Draw(CardOutlineRectangle(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height -160));
            }
            else
            {
                topLeftCorner = new Vector2f((position - 1) * (Card.width + 20) + 410, Game.ScreenHeight - Card.height - 160);
            }
            this.outline = CardOutlineRectangle(topLeftCorner);
        }
        //simply to check if mouse is in the fieldposition
        public Boolean Contains(Vector2f point)
        {
            if(outline.GetGlobalBounds().Contains(point.X, point.Y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(outline);
            if (HasCard)
            {
                target.Draw(card);
            }
        }

        

        //This is a helper to reduce the annoyance of drawing the outlines
        public static RectangleShape CardOutlineRectangle(float x, float y)
        {
           return new RectangleShape(new Vector2f(200f, 320f)) { OutlineColor = Color.Green, OutlineThickness = 1, FillColor = Color.Transparent, Position = new Vector2f { X = x, Y = y } };
        }
        public static RectangleShape CardOutlineRectangle(Vector2f corner)
        {
            return new RectangleShape(new Vector2f { X = 200f, Y = 320f }) { OutlineColor = Color.Green, OutlineThickness = 1, FillColor = Color.Transparent, Position = new Vector2f { X = corner.X, Y = corner.Y } };
        }
    }
}
