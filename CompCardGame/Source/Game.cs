using System;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System.Diagnostics;
using System.Collections.Generic;

namespace CompCardGame.Source
{
    enum GameState
    {
        Loading,
        MainPage,
        Match,
        Settings
    }
    class Game
    {

        private RenderWindow window; //Essentially what you are drawing too

        private Match match;

        public static GameState GameState;

        

        public static InputHandler InputHandler = new InputHandler();
        //private Card selectedCard;
        //drawing the field and positioning of the cards on the field will be dependent on these two
        public static uint ScreenWidth { get; private set; }
        public static uint ScreenHeight { get; private set; }

        private static Stopwatch stopwatch;//used to get time passed
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
            window.MouseMoved += new EventHandler<MouseMoveEventArgs>(InputHandler.MouseMovement);
            //handler for mouseclick
            window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(InputHandler.MouseClick);

            window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(InputHandler.MouseReleased);

            GameState = GameState.Match;
            //temporary this will be later connected to a button on the main page screen
            match = new Match(new Player(PlayerType.Player), new Player(PlayerType.Enemy), window);
            InputHandler.SetMatch(match);

            stopwatch = new Stopwatch();
            stopwatch.Start();
    
        }
        //get current time
        public static TimeSpan GetTimeStamp()
        {
            return stopwatch.Elapsed;
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
            var time = stopwatch.Elapsed;
            InputHandler.Update(time);
            switch (GameState)
            {
                case GameState.Loading:
                    window.Draw(new RectangleShape(new SFML.System.Vector2f(ScreenWidth, ScreenHeight)));
                    break;
                case GameState.MainPage:
                    break;
                case GameState.Match:
                    match.Update();
                    break;
                case GameState.Settings:
                    break;
                default:
                    break;
            }
            //if (!Mouse.IsButtonPressed(Mouse.Button.Left))
            //{
            // player1.ResetCards();
            //}
            //card.Position += new SFML.System.Vector2f
            //{
            //    X = 10f,
            //    Y = 10f
            //};

            //method to draw arrow to mouse from attacking card
        }
        private void Render()
        {
            window.Clear();
            //I think we need to setup a view so that sizing doesn't go weird if you resize the game
            //these are the individual calls to each class for drawing what they need to draw
            //Remember order matters for drawing
            switch(GameState)
            {
                case GameState.Loading:
                    window.Draw(new RectangleShape(new SFML.System.Vector2f(ScreenWidth, ScreenHeight)));
                    break;
                case GameState.MainPage:
                    window.Draw(InputHandler);
                    
                    break;
                case GameState.Match:
                    match.Render();
                    window.Draw(InputHandler);
                    break;
                case GameState.Settings:
                    break;
                default:
                    break;
            }
            //window.Draw(gameField);
            
            //window.Draw(player2);
            //window.Draw(player1);
            //if (selectedCard != null)
            //{
            //    window.Draw(selectedCard);
            //}
            window.Display();
        }

        //this is the event handler method called earlier
        private void OnClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        
        

        


        

        
        
    }
}

    
