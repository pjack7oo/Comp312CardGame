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

namespace Crystal_Wars.Source.Field
{
    enum ViewType
    {
        FieldView,
        SideView
    }

    class Field
    {
        readonly Random  random = new Random();
        //temporary will be fieldPositions in the future
        //Card[] player1Field;
        //Card[] player2Field;
        //FieldPosition[] player1Field;
        //FieldPosition[] player2Field;
        PlayerField player1Field;
        PlayerField player2Field;


        private RenderWindow window;

        private readonly Text turnStateText;
        private readonly Text matchStateText;
        public Field(RenderWindow window)
        {
            this.window = window;
            //player1Field = new FieldPosition[5];
            //player2Field = new FieldPosition[5];
            //int count = 1;
            player1Field = new PlayerField(PlayerType.Player);
            player2Field = new PlayerField(PlayerType.Enemy);
            

            turnStateText = HelperFunctions.NewText(Match.TurnState.ToString(), 15,  new Vector2f { X = 140f, Y = Game.ScreenHeight/2 }, Color.Green);
            matchStateText = HelperFunctions.NewText(Match.MatchState.ToString(), 15, new Vector2f { X = 40f, Y = Game.ScreenHeight / 2 }, Color.Green);
            //initializing fieldPositions
            
            //for (int i = 0; i < 5; i++)
            //{
            //    player1Field[i] = new FieldPosition(count);
            //    count++;
            //}
            //for (int i = 0; i < 5; i++)
            //{
            //    player2Field[i] = new FieldPosition(count);
            //    count++;
            //}
        }
        //might move handling of obj mouse can touch can be moved to its own file
        public Tuple<PlayerType,FieldPosition> GetTarget(Vector2f mouse)
        {
            var target = player1Field.GetTarget(mouse);
            if (target != null)
            {
                return target;
            }
            
            if (Match.TurnState == TurnState.Attack)
            {
                target = player2Field.GetTarget(mouse);
                if (target != null)
                {
                    if (target.Item2.HasCard)
                    {
                        return target;
                    }
                    
                }
            }
            return null;
            
            //for (int i = 0; i < player1Field.Count(); i++)
            //{
            //    if (player1Field[i].Contains(mouse))
            //    {
            //        return new Tuple<PlayerType, FieldPosition>(PlayerType.Player, player1Field[i]);
            //    }
            //}
            
            //for (int i = 0; i < player2Field.Count(); i++)//check if we clicked on opponent field
            //{   if (Match.TurnState == TurnState.Attack)//handle click on opponent in attack phase
            //    {
            //        if (player2Field[i].Contains(mouse) && player2Field[i].HasCard)
            //        {
            //            return new Tuple<PlayerType, FieldPosition>(PlayerType.Enemy, player2Field[i]);
            //        }
            //    }
                
            //}
            //return null;
        }

        //used by cpu to get random spot to place card
        public FieldPosition GetRandomUnusedMonsterFieldPosition()
        {

            return player2Field.GetRandomUnusedMonsterFieldPosition();
            
        }

        public FieldPosition GetRandomUnusedSpellFieldPosition()
        {

            return player2Field.GetRandomUnusedSpellFieldPosition();


        }

        

        public Boolean PlaceCardOnField(PlayerType player,PlayerAction.CardType? cardType, int? fieldPosition, Card card)
        {
            if (player == PlayerType.Player)
            {
                return player1Field.PlaceCardOnField(cardType, fieldPosition, card);
            }
            else //enemy 
            {
                return player2Field.PlaceCardOnField(cardType, fieldPosition, card);
            }
        }

        public Boolean PlaceCardOnField(PlayerType player, FieldPosition fieldPosition, Card card)
        {
            if (player == PlayerType.Player)
            {
                if (!fieldPosition.HasCard)
                {
                    if (card is SpellCard)
                    {
                        card.State = CardState.Back;
                    }
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
                    if (card is MonsterCard)
                    {
                        card.State = CardState.Front;
                    }
                    else
                    {
                        card.State = CardState.Back;
                    }
                    
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

        public Card SelectAnyCard(Vector2f mouse)
        {
            var card = player1Field.SelectCardWithoutActivation(mouse);
            if (card!= null)
            {
                return card;
            }
            card = player2Field.SelectCardWithoutActivation(mouse);
            if (card != null)
            {
                return card;
            }
            return null;
        }

        public void GiveCardsMana(MatchState player)
        {
            if (player == MatchState.Player)
            {
                player1Field.GiveCardsMana();
            }
            else
            {
                player2Field.GiveCardsMana();
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
        //to pass along to playerfield
        public Card SelectPlayerCard(Vector2f mouse)
        {
            return player1Field.SelectCard(mouse);
        }

        public bool PlayerFieldHasUsableSpellCard(PlayerType type)
        {
            if (type == PlayerType.Player)
            {
                return player1Field.HasUsableSpellCard();
            }
            else
            {
                return player2Field.HasUsableSpellCard();
            }
        }
        public bool PlayerFieldHasUsableMonster(PlayerType type)
        {
            if (type == PlayerType.Player)
            {
                return player1Field.HasUsableMonster();
            }
            else
            {
                return player2Field.HasUsableMonster();
            }
        }

        public Card SelectPlayerSpellCard(Vector2f mouse)
        {
            return player1Field.SelectSpellCard(mouse);
        }

        //remove the red outline of any selected card
        public void ResetCardSelection()
        {
            player1Field.ResetCardSelection();
        }

        //this is for the cpu to place on random field pos
        //public FieldPosition GetRandomFieldPosition(PlayerType player)
        //{
            
        //    if (player == PlayerType.Enemy)
        //    {
        //        return player2Field[random.Next(0, player2Field.Length)];
        //    }
        //    return null;
        //}

        //drawing the field positions will also in future handle the background
        public void Draw(ViewType viewType)
        {
            //DrawGrid(viewType);
            if (viewType == ViewType.SideView)
            {
                window.Draw(turnStateText);
                window.Draw(matchStateText);
            }
            else
            {
                player1Field.Draw(window);

                player2Field.Draw(window);
            }
            
            
            

            //target.Draw(CardOutlineRectangle(1400,600));
        }

        public void RemoveCard(Card card)
        {
            player1Field.RemoveCard(card);

            player2Field.RemoveCard(card);
        }
        private void DrawGrid(ViewType viewType)
        {
            if (viewType == ViewType.FieldView)
            {
                for (int i = -350; i < Game.ScreenWidth; i =i+  50) {
                    VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
                    line[0] = new Vertex(new Vector2f(i, -270), Color.White);
                    line[1] = new Vertex(new Vector2f(i, Game.ScreenHeight), Color.White);
                    if (i %100 == 0)
                    {
                        line[0] = new Vertex(new Vector2f(i, -270), Color.Red);
                        line[1] = new Vertex(new Vector2f(i, Game.ScreenHeight), Color.Red);
                    }
                    if (i % 1000 == 0)
                    {
                        line[0] = new Vertex(new Vector2f(i, -270), Color.Green);
                        line[1] = new Vertex(new Vector2f(i, Game.ScreenHeight), Color.Green);
                    }

                    window.Draw(line);
                }
                //for (int i = -250; i < Game.ScreenHeight; i = i + 50)
                //{
                //    VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
                //    line[0] = new Vertex(new Vector2f(-350, i), Color.White);
                //    if (i % 100 == 0)
                //    {
                //        line[0] = new Vertex(new Vector2f(-350, i), Color.Red);
                //    }
                //    if (i % 1000 == 0)
                //    {
                //        line[0] = new Vertex(new Vector2f(Game.ScreenWidth, i), Color.Green);
                //    }
                //    line[1] = new Vertex(new Vector2f(Game.ScreenWidth, i), Color.White);
                    
                //    window.Draw(line);
                //}
            }
            else
            {

            }
        }

        
    }
}
