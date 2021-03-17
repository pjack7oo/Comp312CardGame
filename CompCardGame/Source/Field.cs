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
        readonly Random  random = new Random();
        //temporary will be fieldPositions in the future
        //Card[] player1Field;
        //Card[] player2Field;
        FieldPosition[] player1Field;
        FieldPosition[] player2Field;

        private readonly Text turnStateText;
        private readonly Text matchStateText;
        public Field()
        {
            player1Field = new FieldPosition[5];
            player2Field = new FieldPosition[5];
            int count = 1;

            turnStateText = HelperFunctions.NewText(Match.TurnState.ToString(), 15,  new Vector2f { X = 100f, Y = Game.ScreenHeight/2 }, Color.Green);
            matchStateText = HelperFunctions.NewText(Match.MatchState.ToString(), 15, new Vector2f { X = 1f, Y = Game.ScreenHeight / 2 }, Color.Green);
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
        //might move handling of obj mouse can touch can be moved to its own file
        public Tuple<PlayerType,FieldPosition> GetTarget(Vector2f mouse)
        {
            
            for (int i = 0; i < player1Field.Count(); i++)
            {
                if (player1Field[i].Contains(mouse))
                {
                    return new Tuple<PlayerType, FieldPosition>(PlayerType.Player, player1Field[i]);
                }
            }
            for (int i = 0; i < player2Field.Count(); i++)//check if we clicked on opponent field
            {   if (Match.TurnState == TurnState.Attack)//handle click on opponent in attack phase
                {
                    if (player2Field[i].Contains(mouse) && player2Field[i].HasCard)
                    {
                        return new Tuple<PlayerType, FieldPosition>(PlayerType.Enemy, player2Field[i]);
                    }
                }
                
            }
            return null;
        }

        //used by cpu to get random spot to place card
        public FieldPosition GetRandomUnusedFieldPosition()
        {

            var randomInt = random.Next(0, player2Field.Length);
            if (!player2Field[randomInt].HasCard)
            {
                return player2Field[randomInt];
            }
            else 
            {
                
                foreach(var fieldPos in player2Field)
                {
                    if (!fieldPos.HasCard)
                    {
                        return fieldPos;
                    }
                    
                }
                return null;
            }
            
            
            
        }

        public Boolean PlaceCardOnField(PlayerType player, FieldPosition fieldPosition, Card card)
        {
            if (player == PlayerType.Player)
            {
                if (!fieldPosition.HasCard)
                {
                    fieldPosition.Card = card;
                    card.UpdatePositions();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else //enemy 
            {
                if (!fieldPosition.HasCard)
                {
                    card.State = CardState.Front;
                    fieldPosition.Card = card;
                    card.UpdatePositions();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        
        public void UpdateTurnStateText()
        {
            turnStateText.DisplayedString= Match.TurnState.ToString();
        }

        public void UpdateMatchStateText()
        {
            matchStateText.DisplayedString = Match.MatchState.ToString();
        }

        public Card SelectCard(Vector2f mouse)
        {
            Card result = null;
            foreach(var fieldPos in player1Field)//go through field and select the card that contains mouse
            {
                if (fieldPos.HasCard && fieldPos.Contains(mouse))
                {
                    fieldPos.Card.Active = true;
                    result = fieldPos.Card;
                }
                
            }
            if (result != null)
            {
                foreach(var fieldPos in player1Field)//make not selected cards inactive
                {
                    if (result != fieldPos.Card && fieldPos.HasCard)
                    {
                        fieldPos.Card.Active = false;
                    }
                }
            }
            return result;
        }
        //remove the red outline of any selected card
        public void ResetCardSelection()
        {
            foreach (var fieldPos in player1Field)//make not selected cards inactive
            {
                if (fieldPos.HasCard)
                {
                    fieldPos.Card.Active = false;
                }
            }
        }

        //this is for the cpu to place on random field pos
        public FieldPosition GetRandomFieldPosition(PlayerType player)
        {
            
            if (player == PlayerType.Enemy)
            {
                return player2Field[random.Next(0, player2Field.Length)];
            }
            return null;
        }

        //drawing the field positions will also in future handle the background
        public void Draw(RenderTarget target, RenderStates states)
        {

            target.Draw(turnStateText);
            target.Draw(matchStateText);
            
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
