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

        private Card lastSelectedCard;//used for drawing the zoom up 
        

        

        //this is has the system done an update to the field/cards/player for that match phase
        //will be reset every switch of the turnstate
        private Boolean hasDoneUpdate;
        public Match(Player player1, Player player2, RenderWindow window)
        {
            //initialize field and players by setting positions and drawing 3 cards
            this.window = window;
            players = new Player[2] {player1, player2};
            TurnState = TurnState.Primary;
            hasDoneUpdate = false;
            field = new Field(window);

            foreach (var player in players)
            {
                player.SetDeckPosition();
                for (int i = 0; i < 3; i++)
                {
                    player.DrawACardFromDeck();
                }
            }
            
            
            //add button used by match will need also settings button
            Game.InputHandler.AddButton(new Button("Next Phase",20, new Vector2f(Game.ScreenWidth-400, Game.ScreenHeight / 2), Color.Black,NextTurnState));
        }

        public void RenderSideView()
        {
            field.Draw(ViewType.SideView);

            if (lastSelectedCard != null)
            {
                lastSelectedCard.viewType = ViewType.SideView;
                window.Draw(lastSelectedCard);
            }

            foreach (var player in players)
            {
                player.viewType = ViewType.SideView;
                window.Draw(player);
            }
        }

        public void RenderFieldView()
        {
            field.Draw(ViewType.FieldView);

            if (selectedCard != null)
            {
                selectedCard.viewType = ViewType.FieldView;
                window.Draw(selectedCard);
            }

            foreach (var player in players)
            {
                player.viewType = ViewType.FieldView;
                window.Draw(player);
            }

            
        }
        //go to next turn state and reset has done update and call to update text
        public void NextTurnState()
        {
            TurnState ++;
            if (TurnState > TurnState.End)
            {
                NextMatchState();
                TurnState = 0;
            }
            hasDoneUpdate = false;
            field.UpdateTurnStateText();
            field.ResetCardSelection();

        }
        //next match state and update text
        public void NextMatchState()
        {
            MatchState = 1 - MatchState;//flip between 0 and 1
            field.UpdateMatchStateText();
        }

        //control mouse movement in a match
        public void MouseMovement(Vector2f mouse)
        {
            if (MatchState == MatchState.Player)
            {

            
            switch(TurnState)
            {
                
                default:
                    players[0].HandleMouseMovement(mouse);

                    if (selectedCard != null && selectedCard.Location == CardLocation.Moving)
                    {
                        //needs to be shifted so mouse in middle of the card
                        selectedCard.UpdatePositions(mouse);
                    }
                    break;
            }
            }
            else
            {
                players[0].HandleMouseMovement(mouse);
               
            }

        }

        public void MouseReleased(Vector2f mouse)
        {
            if (MatchState == MatchState.Player)
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
                                players[(int)MatchState].RemoveCrystals(selectedCard.CrystalCost);
                                target.Item2.Card = selectedCard;
                                selectedCard.Location = CardLocation.Field;
                                selectedCard.Active = false;
                                players[0].AddCardToRemoveQueue(selectedCard);
                                //cardsToRemove.Enqueue(selectedCard);
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
                    if (selectedCard != null)
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
            }
            }

        }



        public void MouseClick(Vector2f mouse)
        {

            //check buttons
            var tempForCardZoom = field.SelectAnyCard(mouse);
            if (tempForCardZoom != null)
            {
                lastSelectedCard = tempForCardZoom;
            }
            if (MatchState == MatchState.Player)
            {
                switch (TurnState) //TODO: needs handler for each phase
                {
                    case TurnState.Drawing:
                        selectedCard = players[0].HandleMouseClick(mouse);
                        if (selectedCard != null)
                        {
                            lastSelectedCard = selectedCard;
                        }
                        
                        break;
                    case TurnState.Primary:


                        selectedCard = players[0].HandleMouseClick(mouse);
                        if (selectedCard != null)
                        {
                            lastSelectedCard = selectedCard;
                        }
                        
                        break;
                    case TurnState.Attack:
                        var selectedCard2 = players[0].HandleMouseClickForOppenentTurn(mouse);
                        if (selectedCard2 != null)
                        {
                            lastSelectedCard = selectedCard2;
                        }
                        var temp = field.SelectPlayerCard(mouse);
                        if (temp != null)
                        {
                            selectedCard = temp;
                            if (selectedCard != null)
                            {
                                lastSelectedCard = selectedCard;
                            }
                        }
                        else
                        {
                            Tuple<PlayerType, FieldPosition> target;
                            target = field.GetTarget(mouse);
                            if (target != null && target.Item1 == PlayerType.Enemy && selectedCard != null) //handle attacking card
                            {
                                CalculateCardAttack(selectedCard, target);
                            }
                            //else if (false)//TODO check area on top to attack health of player //attack hp directly
                            //{
                            //    Console.WriteLine("Attack health");
                            //}
                        }

                        break;
                    case TurnState.Secondary:
                        selectedCard = players[0].HandleMouseClick(mouse);
                        if (selectedCard != null)
                        {
                            lastSelectedCard = selectedCard;
                        }
                        break;
                    case TurnState.End:
                        selectedCard = players[0].HandleMouseClick(mouse);
                        if (selectedCard != null)
                        {
                            lastSelectedCard = selectedCard;
                        }
                        break;
                    default:
                        break;

                }
            }
            else
            {
                selectedCard = players[0].HandleMouseClickForOppenentTurn(mouse);
                if (selectedCard != null)
                {
                    lastSelectedCard = selectedCard;
                }
                
            }
            


        }

        public void Update()//multi player match might have to override this
        {
            
            foreach(var player in players)//do player updates that arent based on turn state
            {
                player.Update();
            }
            switch(MatchState)//TODO 
            {
                case MatchState.Player:
                    switch(TurnState)
                    {
                        case TurnState.Drawing://draw a card and give crystals to player and update card mana
                            if (!hasDoneUpdate)
                            {
                                players[(int)MatchState].DrawACardFromDeck();
                                players[(int)MatchState].GetCrystals();

                                hasDoneUpdate = true;
                            }
                            else
                            {

                            }
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
                            if (!hasDoneUpdate)
                            {
                                players[(int)MatchState].DrawACardFromDeck();
                                players[(int)MatchState].GetCrystals();

                                hasDoneUpdate = true;
                            }
                            else
                            {

                            }
                            break;
                        case TurnState.Primary://player can place down cards check if he has crystals, use spells
                            if (!hasDoneUpdate)//temporary computer class will handle this in future
                            {
                                var done = false;
                                while (players[(int)MatchState].Crystals>0 && !done)
                                {
                                    var card = players[(int)MatchState].GrabRandomCard();//temporary later it will be ai playing 
                                                                                         //return a field position if one cannot be found it will tell the CPU to possibly sacrifice to summon
                                                                                         //stronger monters
                                    var fieldPosition = field.GetRandomUnusedFieldPosition();
                                    //for now if field is full it will not play any cards except spells
                                    if (fieldPosition != null)
                                    {
                                        field.PlaceCardOnField(PlayerType.Enemy, fieldPosition, card);
                                        card.Location = CardLocation.Field;
                                        card.Active = false;
                                        players[(int)MatchState].RemoveCrystals(card.CrystalCost);
                                        players[(int)MatchState].AddCardToRemoveQueue(card);
                                        card = null;
                                    } else
                                    {
                                        done = true;
                                    }
                                    
                                }
                                
                                
                                
                                //target.Item2.Card = selectedCard;
                                
                                //players[0].RemoveCard(selectedCard);
                                //card = null;

                                hasDoneUpdate = true;
                            }
                            else
                            {
                                

                            }
                            
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
            
        }

        private void CalculateCardAttack(Card card1, Tuple<PlayerType, FieldPosition> target) // handle attack between cards 
        {
            var card2 = target.Item2.Card;
            Console.WriteLine($"{card1} attacked {card2}");
            if (card1.Mana -card1.attackManaCost>=0)
            {
                if (card1.Attack > card2.Defense)//attack worked, send card2 to graveyard and deal difference of attack - defense to opponent
                {
                    players[(int)target.Item1].SendCardToGraveyard(card2);

                    players[(int)target.Item1].ApplyDamage(card1.Attack - card2.Defense);

                    target.Item2.ResetCard();
                }
                else //attack failed you get dealt difference of defense -attack
                {
                    players[(int)target.Item1 - 1].ApplyDamage(card2.Defense - card1.Attack);
                }
                card1.Mana -= card1.attackManaCost;
            }
            
        }


       
    }
}
