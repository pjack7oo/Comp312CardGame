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
        public static View defaultView;
        public static View fieldView;
        public static View sideView;

        public static uint SideViewWidth { get; private set; }

        public static InputHandler InputHandler;
        //private Card selectedCard;
        //drawing the field and positioning of the cards on the field will be dependent on these two
        public static float ScreenWidth { get; private set; }
        public static float ScreenHeight { get; private set; }

        public static float StretchedScreenWidth { get { return ScreenWidth * 1.5f; }  }
        public static float StretchedScreenHeight { get { return ScreenWidth * 1.5f;}  }

        private static Stopwatch stopwatch;//used to get time passed
        public void Initialize()
        {
            //boiler plate setup for SFML
            ScreenWidth = 1920f;
            ScreenHeight = 1080f;
            var mode = new VideoMode((uint)ScreenWidth,(uint) ScreenHeight);
            window = new RenderWindow(mode, "SFML works!");
            window.SetFramerateLimit(60);
            //this is how events are handled we will probably just need esc on keyboard and mouse movement/clicking
            window.Closed += new EventHandler(OnClose);
            //handler for mouseMovement
            InputHandler = new InputHandler(window);
            window.MouseMoved += new EventHandler<MouseMoveEventArgs>(InputHandler.MouseMovement);
            //handler for mouseclick
            window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(InputHandler.MouseClick);

            window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(InputHandler.MouseReleased);

            window.Resized += new EventHandler<SizeEventArgs>(OnResize);

            fieldView = new View(new FloatRect(0f, 0f, ScreenWidth, ScreenHeight));
            fieldView.Zoom(1.5f);
            fieldView.Viewport = new FloatRect(0.2f, 0f, 1f, 1f);

            defaultView = new View(new FloatRect(0f, 0f, ScreenWidth, ScreenHeight));//used on main screen

            sideView = new View(new FloatRect(0f, 0f, ScreenWidth/4.8f, ScreenHeight));
            sideView.Zoom(0.8f);
            sideView.Viewport = new FloatRect(0f, 0f, 0.2f, 1f);
            SideViewWidth = 400;

            //window.SetView(fieldView);//for testing views

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
                    window.SetView(defaultView);
                    window.Draw(new RectangleShape(new SFML.System.Vector2f(ScreenWidth, ScreenHeight)));
                    break;
                case GameState.MainPage:
                    window.SetView(defaultView);
                    window.Draw(InputHandler);
                    
                    break;
                case GameState.Match:
                    window.SetView(fieldView);//draw field 
                    match.RenderFieldView();
                    window.Draw(InputHandler);

                    window.SetView(sideView);//draw sideview like zoom up of cards and display stats
                    match.RenderSideView();

                    break;
                case GameState.Settings:
                    window.SetView(defaultView);
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
        //resize window if size changed //todo go through things and make sure all the positions are updated and positions should all be relattive to size
        private void OnResize(object sender, SizeEventArgs e)
        {
            fieldView = new View(new FloatRect(0f, 0f, e.Width, e.Height));
            fieldView.Zoom(1.5f);
            fieldView.Viewport = new FloatRect(0.2f, 0f, 1f, 1f);
            //window.SetView(new View(visibleArea));
            defaultView = new View(new FloatRect(0f, 0f, e.Width, e.Height));//used on main screen

            sideView = new View(new FloatRect(0f, 0f, e.Width/4.8f, e.Height));
            sideView.Zoom(0.8f);
            sideView.Viewport = new FloatRect(0f, 0f, 0.2f, 1f);
            SideViewWidth = 400;
        }
        
        

        


        

        
        
    }
}

    
