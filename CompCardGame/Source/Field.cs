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
        //temporary will be fieldPositions in the future
        //Card[] player1Field;
        //Card[] player2Field;
        FieldPosition[] player1Field;
        FieldPosition[] player2Field;
        public Field()
        {
            player1Field = new FieldPosition[5];
            player2Field = new FieldPosition[5];
            int count = 1;
            //initializing fieldPositions
            for (int i = 0; i < 5; i++)
            {
                player1Field[i] = new FieldPosition(count);
                count++;
            }
            for (int i = 0; i < 5; i++)
            {
                player2Field[i] = new FieldPosition(count);
                count++;
            }
        }

        //not done since cant move cards yet
        public Boolean placeCardOnField(int player, int fieldPosition, Card card)
        {
            if (player == 1)
            {
                if (!player1Field[fieldPosition].HasCard)
                {
                    player1Field[fieldPosition].card = card;
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

        //drawing the field positions will also in future handle the background
        public void Draw(RenderTarget target, RenderStates states)
        {
            
            for (int i = 0; i < player1Field.Count(); i++)
            {
                target.Draw(player1Field[i]);
                //target.Draw(CardOutlineRectangle(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height -160));
            }

            for (int i = 0; i < player2Field.Count(); i++)
            {
                target.Draw(player2Field[i]);
                //target.Draw(new RectangleShape(new SFML.System.Vector2f { X = 200f, Y = 320f }) { OutlineColor = Color.Green, OutlineThickness = 1, FillColor = Color.Transparent, Position = new SFML.System.Vector2f { X = i * 220 + 200, Y = 100 } });
                //target.Draw(CardOutlineRectangle(i * (Card.width + 20) + 410, 160));
            }

            //target.Draw(CardOutlineRectangle(1400,600));
        }
    }
}
