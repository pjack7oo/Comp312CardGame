﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Crystal_Wars.Source.Core;

namespace Crystal_Wars.Source
{
    class Start
    {
        //start game
        static void Main(String[] args)
        {
            FileHandler.LoadAllImages();
            Game game = new Game();
            game.Initialize();

            
            Database.DBConnectionTest();

            
            
            game.Run();

            
        }


        
    }
}
