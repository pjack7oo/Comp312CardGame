using System;
using SFML.Window;
using SFML.Graphics;
using System.Collections.Generic;

namespace CompCardGame.Source
{
    class Game
    {

        RenderWindow window;
        List<Card> cards = new List<Card>();
        public void Start()
        {
            var mode = new VideoMode(1920, 1080);
            window = new RenderWindow(mode, "SFML works!");
            window.Closed += new EventHandler(OnClose);
            window.SetFramerateLimit(60);
            cards.Add(new Card("Card 1", "First Card I have", 100, 200) { Position = new SFML.System.Vector2f { X = 100,Y= 750 } });
            cards.Add(new Card("Card 2", "Second Card I have", 100, 200) { Position = new SFML.System.Vector2f { X = 400, Y = 750 } });
            //temp


        }
        public void Run()
        {
            while (window.IsOpen)
            {
                window.DispatchEvents();
                ProcessEvents();
                Update();
                Render();
            }
        }

        private void ProcessEvents()
        {
            //Event @event;
            //while(window.pollEvent(@e))

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
            //var vector = new SFML.System.Vector2f { X = 100f, Y = 100f };
            //var shape = new SFML.Graphics.RectangleShape(vector) { FillColor = SFML.Graphics.Color.Cyan };
            //shape.Position = new SFML.System.Vector2f { X = 100, Y = 100};
            foreach(var card in cards)
            {
                window.Draw(card);
            }
            
            window.Display();
        }

        private void OnClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }
    }
}

    
