using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompCardGame.Source.Objects;

namespace CompCardGame.Source.Core
{
    class CardManager
    {
        private RenderWindow window;

        private Text title;
        private Player player;
        public CardManager(RenderWindow renderWindow, Player player)
        {
            window = renderWindow;
            title = HelperFunctions.NewText("Card and Deck Manager", 40, new Vector2f(Game.ScreenWidth / 2, 20), new Color(101, 67, 33));
            this.player = player;
        }


        public void Render()
        {

        } 
    }
}
