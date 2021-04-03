using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompCardGame.Source.Core;

namespace CompCardGame.Source
{
    class Start
    {
        //start game
        static void Main(String[] args)
        {
            Game game = new Game();
            game.Initialize();

            //for listing databases test
            Database test = new Database();
            test.DBConnectionTest();

            game.Run();
        }
    }
}
