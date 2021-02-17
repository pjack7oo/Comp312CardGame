using System;
using SFML.Window;
using SFML.Graphics;
using System.Collections.Generic;

namespace CompCardGame.Source
{


    class Game
    {

        RenderWindow window; //Essentially what you are drawing too
        List<Card> cards = new List<Card>();//temporary

        Player player1;
        Player player2;
        Field gameField;

        private Card selectedCard;
        //drawing the field and positioning of the cards on the field will be dependent on these two
        public static uint ScreenWidth { get; private set; }
        public static uint ScreenHeight { get; private set; }

        public void Initialize()
        {
            //boiler plate setup for SFML
            ScreenWidth = 1920;
            ScreenHeight = 1080;
            var mode = new VideoMode(ScreenWidth, ScreenHeight);
            window = new RenderWindow(mode, "SFML works!");
            window.SetFramerateLimit(60);
            //this is how events are handled we will probably just need esc on keyboard and mouse movement/clicking
            window.Closed += new EventHandler(OnClose);
            //handler for mouseMovement
            window.MouseMoved += new EventHandler<MouseMoveEventArgs>(MouseMovement);
            //handler for mouseclick
            window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(MouseClick);

            window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(MouseReleased);
            //just initializing the fields, this will be moved into seperate code for starting a match
            player1 = new Player(PlayerType.Player);
            player2 = new Player(PlayerType.Enemy);
            gameField = new Field();
            player1.SetDeckPosition();
            player2.SetDeckPosition();
            
            //draw 3 cards from the deck and add to hand
            for(int i = 0; i < 3;i++)
            {
                player1.DrawACardFromDeck();
                player2.DrawACardFromDeck();
            }

        }
        public void Run()
        {
            //boilerplate gameloop
            // still needs timing to base things off so it lags less if we ever get to a problem of fps
            while (window.IsOpen)
            {
                window.DispatchEvents();
                
                Update();//will need timing
                Render();
            }
        }
        
       

        private void Update()
        {
            //if (!Mouse.IsButtonPressed(Mouse.Button.Left))
            //{
            // player1.ResetCards();
            //}
            //card.Position += new SFML.System.Vector2f
            //{
            //    X = 10f,
            //    Y = 10f
            //};

        }
        private void Render()
        {
            window.Clear();
            //I think we need to setup a view so that sizing doesn't go weird if you resize the game
            //these are the individual calls to each class for drawing what they need to draw
            //Remember order matters for drawing
            window.Draw(gameField);
            
            window.Draw(player2);
            window.Draw(player1);
            window.Display();
        }

        //this is the event handler method called earlier
        private void OnClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }
        //handles mouseMovement
        private void MouseMovement(object sender, MouseMoveEventArgs e)
        {
            var mouse = new SFML.System.Vector2f(e.X, e.Y);
            player1.HandleMouseMovement(mouse);
            //TODO add handler for when card is selected and being moved
            //if when mouse is clicked and moving a card
            if (selectedCard != null)
            {
                //needs to be shifted so mouse in middle of the card
                selectedCard.Position = mouse;
            }
        }

        //this will be important for handling when card is placed on field and when attacking opponent
        private void MouseReleased(object sender, MouseButtonEventArgs e)
        {
            //i wasn't sure if it was glitching so i made it run separate in a thread
            System.Threading.Thread release = new System.Threading.Thread(new System.Threading.ThreadStart(() => 
            {
                //System.Threading.Thread.Sleep(5);
                Tuple<PlayerType, FieldPosition> target;
                var mouse = new SFML.System.Vector2f(e.X, e.Y);

                target = gameField.GetTarget(mouse);

                if (selectedCard != null && target != null)
                {
                    //handle clicking on player field position this will include spell handling
                    if (target.Item1 == PlayerType.Player)
                    {
                        if (!target.Item2.HasCard)
                        {
                            target.Item2.Card = selectedCard;
                            selectedCard.Location = CardLocation.Field;
                            selectedCard.Active = false;
                            player1.RemoveCard(selectedCard);
                            selectedCard = null;
                            
                            
                        }
                    } 
                    else //handle clicking on opponent field
                    {

                    }
                }
                else if (selectedCard != null && target == null)
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
            }));
            release.Start();
            
            //check where to place card when mouse is released and if no valid slot is selected then drop the card back to previous spot
        }


        
        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                //TODO add handler for player1
                //Console.WriteLine($"{e.X}, {e.Y}");

                selectedCard = player1.HandleMouseClick(new SFML.System.Vector2f(e.X, e.Y));
                if (selectedCard != null)
                {
                    selectedCard.Location = CardLocation.Moving;
                }
                
            }
        }
    }
}

    
