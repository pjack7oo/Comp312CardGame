using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace CompCardGame.Source
{

    #region StateMachine Enumerators
    enum TurnState //this is so we can make state machine so we know which turn phase it is
    {
        Drawing,
        Primary,
        Attack,
        Secondary,
        End
    }

    enum MatchState // this is so we can make a state machine so we know whos turn it is
    {
        Player,
        Opponent
    }
    #endregion
    class Match
    {

        
        private RenderWindow window;
        
        private readonly Player[] players;
        private readonly Field field;

        public static TurnState TurnState;
        public static MatchState MatchState;

        private Card selectedCard;
        private Queue<Card> cardsToRemove = new Queue<Card>();

        public Match(Player player1, Player player2, RenderWindow window)
        {
            //initialize field and players by setting positions and drawing 3 cards
            this.window = window;
            players = new Player[2] {player1, player2};
            field = new Field();

            foreach (var player in players)
            {
                player.SetDeckPosition();
                for (int i = 0; i < 3; i++)
                {
                    player.DrawACardFromDeck();
                }
            }
            field.PlaceCardOnField(PlayerType.Enemy, field.GetRandomFieldPosition(PlayerType.Enemy), player2.GrabRandomCard());//temporary
            TurnState = TurnState.Primary;//temporary

            Game.InputHandler.AddButton(new Button("test Button",20, new Vector2f(1920 / 2-100, 1080 / 2-50), 200, 100, Color.Black));
        }

        public void Render()
        {
            window.Draw(field);
            
            foreach(var player in players)
            {
                window.Draw(player);
            }

            if (selectedCard != null)
            {
                window.Draw(selectedCard);
            }
        }


        public void MouseMovement(Vector2f mouse)
        {
            players[0].HandleMouseMovement(mouse);

            if (selectedCard != null && selectedCard.Location == CardLocation.Moving)
            {
                //needs to be shifted so mouse in middle of the card
                selectedCard.UpdatePositions(mouse);
            }
        }

        public void MouseReleased(Vector2f mouse)
        {
            switch(TurnState)
            {
                case TurnState.Primary:
                    Tuple<PlayerType, FieldPosition> target = field.GetTarget(mouse);

                    if (selectedCard != null && target != null)//
                    {
                        //handle releasing on player field position this will include spell handling
                        if (target.Item1 == PlayerType.Player)
                        {
                            if (!target.Item2.HasCard && selectedCard.Location == CardLocation.Moving)
                            {
                                field.PlaceCardOnField(target.Item1, target.Item2, selectedCard);
                                target.Item2.Card = selectedCard;
                                selectedCard.Location = CardLocation.Field;
                                selectedCard.Active = false;
                                cardsToRemove.Enqueue(selectedCard);
                                //players[0].RemoveCard(selectedCard);
                                selectedCard = null;


                            }
                        }
                        else //handle release on opponent field in necessary
                        {

                        }
                    }
                    else if (selectedCard != null && target == null) //check where to place card when mouse is released and if no valid slot is selected then drop the card back to previous spot
                    {
                        if (selectedCard.Location == CardLocation.Moving)
                        {
                            selectedCard.Active = false;
                            selectedCard.Location = CardLocation.Hand;
                            //player1.ResetCardPosition(selectedCard);
                            selectedCard.ResetCard();
                            selectedCard = null;
                            //Console.WriteLine("released");

                            //player1.ResetCards();
                        }

                    }
                    break;

                default:
                    break;
            }
            

        }

        public void MouseClick(Vector2f mouse)
        {
            
            //check buttons


            switch (TurnState) //TODO: needs handler for each phase
            {
                case TurnState.Drawing:
                    break;
                case TurnState.Primary:
                    
                    
                    selectedCard = players[0].HandleMouseClick(mouse);
                   
                    break;
                case TurnState.Attack:
                    var temp = field.SelectCard(mouse);
                    if (temp != null)
                    {
                        selectedCard = temp;
                    }
                    else
                    {
                        Tuple<PlayerType, FieldPosition> target;
                        target = field.GetTarget(mouse);
                        if (target != null && target.Item1 == PlayerType.Enemy && selectedCard != null) //handle attacking card
                        {
                            CalculateCardAttack(selectedCard, target.Item2.Card);
                        }
                        else if (false)//TODO check area on top to attack health of player //attack hp directly
                        {
                            Console.WriteLine("Attack health");
                        }
                    }
                    
                    break;
                case TurnState.Secondary:
                    break;
                case TurnState.End:
                    break;
                default:
                    break;

            }


        }

        public void Update()//multi player match might have to override this
        {

            switch(MatchState)//TODO 
            {
                case MatchState.Player:
                    switch(TurnState)
                    {
                        case TurnState.Drawing://draw a card and give crystals to player
                            break;
                        case TurnState.Primary://player can place down cards check if he has crystals, use spells
                            break;
                        case TurnState.Attack://player can attack, use spells
                            break;
                        case TurnState.Secondary://player can use spells
                            break;
                        case TurnState.End://end turn apply status effects/field effects possibly
                            break;
                        default:
                            break;
                    }
                    break;
                case MatchState.Opponent://this is for regular match so we need AI code here so will need an AI class
                    switch (TurnState)
                    {
                        case TurnState.Drawing://draw a card and give crystals to player

                            break;
                        case TurnState.Primary://player can place down cards check if he has crystals, use spells

                            break;
                        case TurnState.Attack://player can attack, use spells

                            break;
                        case TurnState.Secondary://player can use spells

                            break;
                        case TurnState.End://end turn apply status effects/field effects possibly

                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            if (cardsToRemove.Count >0)
            {
                while (cardsToRemove.Count >0)
                {
                    var card = cardsToRemove.Dequeue();
                    players[0].RemoveCard(selectedCard);
                }
            }
        }

        private void CalculateCardAttack(Card card1, Card card2) // handle attack between cards 
        {
            Console.WriteLine($"{card1} attacked {card2}");
            if (card1.Attack > card2.Defense)//attack worked, send card2 to graveyard and deal difference of attack - defense to opponent
            {

            }
            else //attack failed you get dealt difference of defense -attack
            {

            }
        }


        private void NextTurnState()
        {
            TurnState++;
        }
    }
}
