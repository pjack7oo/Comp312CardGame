using System;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{
    class Field : Drawable
    {
        Card[] player1Field;
        Card[] player2Field;

        public Field()
        {
            player1Field = new Card[5];
            player2Field = new Card[5];
        }

        public Boolean placeCardOnField(int player, int fieldPosition, Card card)
        {
            if (player == 1)
            {
                if (player1Field[fieldPosition] == null)
                {
                    player1Field[fieldPosition] = card;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else //todo
            {
                if (player2Field[fieldPosition] == null)
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            for (int i = 0; i < player1Field.Count(); i++)
            {
                target.Draw(CardOutlineRectangle(i * 220 + 200, 600));
            }

            for (int i = 0; i < player2Field.Count(); i++)
            {
                //target.Draw(new RectangleShape(new SFML.System.Vector2f { X = 200f, Y = 320f }) { OutlineColor = Color.Green, OutlineThickness = 1, FillColor = Color.Transparent, Position = new SFML.System.Vector2f { X = i * 220 + 200, Y = 100 } });
                target.Draw(CardOutlineRectangle(i * 220 + 200, 100));
            }

            target.Draw(CardOutlineRectangle(1400,600));
        }

        //This is a helper to reduce the annoyance of drawing the outlines
        private static RectangleShape CardOutlineRectangle(float x, float y)
        {
            return new RectangleShape(new Vector2f { X = 200f, Y = 320f }) { OutlineColor = Color.Green, OutlineThickness = 1, FillColor = Color.Transparent, Position = new Vector2f { X = x, Y = y } };
        }
    }
}
