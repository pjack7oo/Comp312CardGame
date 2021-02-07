using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{
    class Start
    {
        //start game
        static void Main(String[] args)
        {
            Game game = new Game();
            game.Initialize();
            game.Run();
        }
    }
}
