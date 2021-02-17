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
            window.MouseMoved += MouseMovement;
            //handler for mouseclick
            window.MouseButtonPressed += MouseClick;

            window.MouseButtonReleased += MouseReleased;
            //just initializing the fields, this will be moved into seperate code for starting a match
            player1 = new Player(PlayerType.Player);
            player2 = new Player(PlayerType.Enemy);
            gameField = new Field();
            player1.setDeckPosition();
            player2.setDeckPosition();
            
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
            window.Draw(player1);
            window.Draw(player2);
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
        }

        //this will be important for handling when card is placed on field and when attacking opponent
        private void MouseReleased(object sender, MouseButtonEventArgs e)
        {

        }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                //TODO add handler for player1
                Console.WriteLine($"{e.X}, {e.Y}");
                //player1.HandleMouseClick()
            }
        }
    }
}

    
