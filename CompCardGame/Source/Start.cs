﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{
    class Start
    {
        static void Main(String[] args)
        {
            Game game = new Game();
            game.Start();
            game.Run();
        }
    }
}
