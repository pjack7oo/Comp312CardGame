using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using CompCardGame.Source.Objects;
using CompCardGame.Source.Field;
using System.Threading;

namespace CompCardGame.Source.Core
{
    class NetworkMatch : Match
    {
        private OnlineState onlineState;
        private Thread serverThread;
        private enum OnlineState
        {
            Setup,
            Waiting,
            Playing
        }
        public NetworkMatch(Player self, RenderWindow window) : base(self, window)
        {
            SetupButtonsForConnection();
            onlineState = OnlineState.Setup;
        }

        private void StartMatch()
        {
            Console.WriteLine("server");
            serverThread = new Thread(new ThreadStart(() =>
            {
                Networking.Start();
            }));
            serverThread.Start();
            SetupButtonsForWaiting();
            onlineState = OnlineState.Waiting;
            
        }

        private void ConnectToMatch()
        {
            Thread connectionThread = new Thread(new ThreadStart(() =>
            {
                Networking.Connect("127.0.0.1", "hello");
            }));
            connectionThread.Start();
        }

        private void SetupButtonsForConnection()
        {
            Game.InputHandler.ClearButtons();
            Game.InputHandler.AddButton(new Button("Start Server", 15, new Vector2f(Game.ScreenWidth / 2, Game.ScreenHeight / 2), Color.Black, StartMatch, new Vector2f(1.25f, 1.25f)));
            Game.InputHandler.AddButton(new Button("Connect Server", 15, new Vector2f(Game.ScreenWidth / 2, Game.ScreenHeight / 2 + 100), Color.Black, ConnectToMatch, new Vector2f(1.25f, 1.25f)));
            Game.InputHandler.AddButton(new Button("Exit", 40, new Vector2f(Game.ScreenWidth / 2, Game.ScreenHeight / 2 + 200), Color.Black, () => { Game.InitiallizeMainPage(); Game.GameState = GameState.MainPage; }));
        }

        private void SetupButtonsForWaiting()
        {
            Game.InputHandler.ClearButtons();
            
            Game.InputHandler.AddButton(new Button("Exit", 40, new Vector2f(Game.ScreenWidth / 2, Game.ScreenHeight / 2 + 200), Color.Black, () => { SetupButtonsForConnection(); onlineState = OnlineState.Setup; Networking.EndSeverConnection(); serverThread.Abort(); }));
        }



        public override void Update(System.TimeSpan time)
        {
            switch (onlineState)
            {
                case OnlineState.Setup:

                    break;
                case OnlineState.Waiting:

                    break;
                case OnlineState.Playing:
                    break;
                default:
                    break;
            }
        }

        public override void Render()
        {
            switch(onlineState)
            {
                case OnlineState.Setup:
                    window.SetView(Game.defaultView);
                    window.Draw(Game.InputHandler);
                    break;
                case OnlineState.Waiting:
                    window.SetView(Game.defaultView);
                    window.Draw(Game.InputHandler);
                    break;
                case OnlineState.Playing:
                    base.Render();
                    break;
                default:
                    break;
            }
        }
    }
}
