using System;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System.Diagnostics;
using System.Collections.Generic;
using CompCardGame.Source.Objects;


namespace CompCardGame.Source.Core
{
    enum GameState
    {
        Loading,
        MainPage,
        CardManager,
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

        public static float StretchedScreenWidth { get { return ScreenWidth * 1.5f; } }
        public static float StretchedScreenHeight { get { return ScreenWidth * 1.5f; } }

        private static Stopwatch stopwatch;//used to get time passed

        private CardManager cardManager;
        public void Initialize()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            //boiler plate setup for SFML
            ScreenWidth = 1920f;
            ScreenHeight = 1080f;
            var mode = new VideoMode((uint)ScreenWidth, (uint)ScreenHeight);
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

            sideView = new View(new FloatRect(0f, 0f, ScreenWidth / 4.8f, ScreenHeight));
            sideView.Zoom(0.8f);
            sideView.Viewport = new FloatRect(0f, 0f, 0.2f, 1f);
            SideViewWidth = 400;

            //window.SetView(fieldView);//for testing views

            GameState = GameState.Loading;
            //temporary this will be later connected to a button on the main page screen
            //match = new Match(new Player(PlayerType.Player), new Player(PlayerType.Enemy), window);
            var player = new Player(PlayerType.Player);
            cardManager = new CardManager(window, player);
            //InputHandler.SetMatch(match);

            InitiallizeLoadingShapes();


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
                    if (time > TimeSpan.FromSeconds(5))
                    {
                        GameState = GameState.MainPage;
                        InitiallizeMainPageShapes();
                        InitiallizeMainPage();
                    }
                    break;
                case GameState.MainPage:
                    break;
                case GameState.CardManager:
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
            switch (GameState)
            {
                case GameState.Loading:
                    window.SetView(defaultView);
                    window.Draw(new RectangleShape(new Vector2f(ScreenWidth, ScreenHeight)) { FillColor = Color.White });
                    DrawLoadingScreen();
                    break;
                case GameState.MainPage:
                    window.SetView(defaultView);
                    DrawMainPageScreen();
                    window.Draw(InputHandler);

                    break;
                case GameState.Match:
                    window.SetView(fieldView);//draw field 
                    match.RenderFieldView();
                    window.Draw(InputHandler);

                    window.SetView(sideView);//draw sideview like zoom up of cards and display stats
                    match.RenderSideView();

                    break;
                case GameState.CardManager:
                    cardManager.Render();
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

            sideView = new View(new FloatRect(0f, 0f, e.Width / 4.8f, e.Height));
            sideView.Zoom(0.8f);
            sideView.Viewport = new FloatRect(0f, 0f, 0.2f, 1f);
            SideViewWidth = 400;
        }

        private void InitiallizeMainPage()
        {
            InputHandler.ClearButtons();
            InputHandler.AddButton(new Button("Play", 20, new Vector2f(ScreenWidth / 2, ScreenHeight / 2), Color.Black, InitiallizeMatch, new Vector2f(1.25f, 1.25f)));
            InputHandler.AddButton(new Button("Online", 20, new Vector2f(ScreenWidth / 2, ScreenHeight / 2 + 100), Color.Black, InitiallizeOnlineMatchPage, new Vector2f(1.25f, 1.25f)));

            InputHandler.AddButton(new Button("Card Manager", 20, new Vector2f(ScreenWidth / 2, ScreenHeight / 2 + 200), Color.Black, InitiallizeCardManager, new Vector2f(1.25f, 1.25f)));
            InputHandler.AddButton(new Button("Settings", 20, new Vector2f(ScreenWidth / 2, ScreenHeight / 2 + 300), Color.Black, InitiallizeSettingsPage, new Vector2f(1.25f, 1.25f)));
            InputHandler.AddButton(new Button("Exit", 20, new Vector2f(ScreenWidth / 2, ScreenHeight / 2 + 400), Color.Black, Exit, new Vector2f(1.25f, 1.25f)));
        }

        private void Exit()
        {
            //TODO destroy everything

            window.Close();
        }

        private void InitiallizeSettingsPage()
        {
            Console.WriteLine("TODO Settings");
        }

        private void InitiallizeOnlineMatchPage()
        {
            Console.WriteLine("TODO Online Match");
        }

        private void InitiallizeCardManager()
        {
            InputHandler.ClearButtons();
            Console.WriteLine("TODO Card Manager");
            
            GameState = GameState.CardManager;
        }

        private void InitiallizeMatch()
        {
            InputHandler.ClearButtons();

            match = new Match(new Player(PlayerType.Player), new Player(PlayerType.Enemy), window);

            InputHandler.SetMatch(match);

            GameState = GameState.Match;
        }

        #region LoadingPageDrawing 
        private Shape[] loadingShapes = new Shape[3];
        private Text[] loadingText = new Text[2];
        private void InitiallizeLoadingShapes()
        {
            loadingShapes[0] = new RectangleShape(new Vector2f(Card.width, Card.height)) { FillColor = Color.Cyan };
            loadingShapes[0].Origin = new Vector2f(Card.width / 2, Card.height / 2);
            loadingShapes[0].Position = new Vector2f(ScreenWidth / 2, ScreenHeight / 2);
            loadingShapes[1] = new RectangleShape(new Vector2f(Card.width, Card.height)) { FillColor = Color.Cyan };
            loadingShapes[1].Origin = new Vector2f(Card.width / 2, Card.height / 2 - 50);
            loadingShapes[1].Position = new Vector2f(ScreenWidth / 2 - 200, ScreenHeight / 2);
            loadingShapes[1].Rotation = -45;
            loadingShapes[2] = new RectangleShape(new Vector2f(Card.width, Card.height)) { FillColor = Color.Cyan };
            loadingShapes[2].Origin = new Vector2f(Card.width / 2, Card.height / 2 - 50);
            loadingShapes[2].Position = new Vector2f(ScreenWidth / 2 + 200, ScreenHeight / 2);
            loadingShapes[2].Rotation = 45;
            loadingText[0] = new Text("Crystal Wars", HelperFunctions.font, 50) { FillColor = Color.Magenta };
            loadingText[0].Position = HelperFunctions.GetCenteredPosition(new Vector2f(ScreenWidth, ScreenHeight), new Vector2f(loadingText[0].GetGlobalBounds().Width, loadingText[0].GetGlobalBounds().Height));
            loadingText[1] = new Text("Loading...", HelperFunctions.font, 50) { FillColor = Color.Black };
            loadingText[1].Position = HelperFunctions.GetCenteredPosition(new Vector2f(ScreenWidth, ScreenHeight + ScreenHeight - 200), new Vector2f(loadingText[1].GetGlobalBounds().Width, loadingText[1].GetGlobalBounds().Height));
        }

        private void DrawLoadingScreen()
        {
            foreach (var shape in loadingShapes)
            {
                window.Draw(shape);
            }
            foreach (var text in loadingText)
            {
                window.Draw(text);
            }
        }




        #endregion

        #region MainPageDrawing
        //private Shape[] mainPageShapes = new Shape[3];
        private Text mainPageText;

        private void InitiallizeMainPageShapes()
        {
            //loadingShapes[0] = new RectangleShape(new Vector2f(Card.width, Card.height)) { FillColor = Color.Cyan };
            //loadingShapes[0].Origin = new Vector2f(Card.width / 2, Card.height / 2);
            //loadingShapes[0].Position = new Vector2f(ScreenWidth / 2, ScreenHeight / 2);


            mainPageText = new Text("Crystal Wars", HelperFunctions.font, 70) { FillColor = Color.Magenta };
            mainPageText.Position = HelperFunctions.GetCenteredPosition(new Vector2f(ScreenWidth, ScreenHeight - 400), new Vector2f(mainPageText.GetGlobalBounds().Width, mainPageText.GetGlobalBounds().Height));
            //loadingText[1] = new Text("Loading...", HelperFunctions.font, 50) { Color = Color.Black };
            //loadingText[1].Position = HelperFunctions.GetCenteredPosition(new Vector2f(ScreenWidth, ScreenHeight + ScreenHeight / 2), new Vector2f(loadingText[1].GetGlobalBounds().Width, loadingText[1].GetGlobalBounds().Height));
        }

        private void DrawMainPageScreen()
        {
            //foreach (var shape in loadingShapes)
            //{
            //    window.Draw(shape);
            //}

            window.Draw(mainPageText);

        }
        #endregion

    }
}


