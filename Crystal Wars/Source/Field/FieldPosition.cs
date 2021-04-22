using System;
using SFML.System;
using SFML.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystal_Wars.Source.Objects;
using Crystal_Wars.Source.Core;

namespace Crystal_Wars.Source.Field
{
    public enum FieldType
    {
        Monster,
        Spell
    }
    class FieldPosition : Drawable
    {
        private Card card;
        public FieldType fieldType;
        public Card Card { get { return card; } set { card = value; card.Position = topLeftCorner; } }
        public int position; //temporary way of store card position it might be changed
        public Boolean HasCard { get { return (this.card != null) ? true : false; } }
        //important for drawing things to the right spot
        private Vector2f topLeftCorner;
        private RectangleShape outline;


        public FieldPosition(PlayerType playerType, int position, FieldType fieldType)
        {
            this.fieldType = fieldType;
            if (fieldType == FieldType.Monster)
            {
                this.position = position;
                //setting the corner for top field and bottom field
                if (playerType == PlayerType.Enemy)
                {
                    topLeftCorner = new Vector2f((position - 6) * (Card.width + 20) + 200, 170);
                    //target.Draw(CardOutlineRectangle(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height -160));
                }
                else//player
                {
                    topLeftCorner = new Vector2f((position - 1) * (Card.width + 20) + 200, Game.ScreenHeight - Card.height - 170);
                }
                this.outline = CardOutlineRectangle(topLeftCorner);
            }
            else
            {
                this.position = position;
                //setting the corner for top field and bottom field
                if (playerType == PlayerType.Enemy)
                {
                    topLeftCorner = new Vector2f((position - 6) * (Card.width + 20) + 200, -170);
                    //target.Draw(CardOutlineRectangle(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height -160));
                }
                else//player
                {
                    topLeftCorner = new Vector2f((position - 1) * (Card.width + 20) + 200, Game.ScreenHeight - 150);
                }
                this.outline = CardOutlineRectangle(topLeftCorner);
            }

        }
        //simply to check if mouse is in the fieldposition
        public Boolean Contains(Vector2f point)
        {
            return outline.GetGlobalBounds().Contains(point.X, point.Y);
            
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(outline);
            if (HasCard)
            {
                card.viewType = ViewType.FieldView;
                target.Draw(card);
            }
        }

        public void ResetCard()
        {
            card = null;
        }

        //This is a helper to reduce the annoyance of drawing the outlines
        public static RectangleShape CardOutlineRectangle(float x, float y)
        {
            return new RectangleShape(new Vector2f(200f, 320f)) { OutlineColor = Color.Green, OutlineThickness = 2, FillColor = Color.Transparent, Position = new Vector2f { X = x, Y = y } };
        }
        public static RectangleShape CardOutlineRectangle(Vector2f corner)
        {
            return new RectangleShape(new Vector2f { X = 200f, Y = 320f }) { OutlineColor = Color.Green, OutlineThickness = 2, FillColor = Color.Transparent, Position = new Vector2f { X = corner.X, Y = corner.Y } };
        }
    }
}
