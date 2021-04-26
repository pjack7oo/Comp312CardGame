using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Crystal_Wars.Source.Core
{
    class Input : Drawable
    {
        private RectangleShape box;
        private Text playerText;
        private string playerInput;
        public bool TakeInput { get; set; }
        public Input(Vector2f pos)
        {
            
            box = new RectangleShape(new Vector2f(300,50)) { Origin = new Vector2f(150,25), Position = pos };
            playerText = HelperFunctions.NewText("",20,new Vector2f(pos.X - 150, pos.Y - 25),Color.Black);
            TakeInput = false;
            playerInput = "";
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(box);
            target.Draw(playerText);
        }

        public void GetInput(TextEventArgs e)
        {
            if (TakeInput)
            {
                if (e.Unicode == "\b")
                {
                    if (playerInput.Length >0)
                    {
                        playerInput = playerInput.Remove(playerInput.Length - 1);
                        playerText.DisplayedString = playerText.DisplayedString.Remove(playerText.DisplayedString.Length - 1);
                        Console.WriteLine(playerInput);
                    }
                   
                    return;
                }
                else if  (playerInput.Length <=15)
                {
                    
                    playerInput += e.Unicode;
                    playerText.DisplayedString = playerInput;
                }

            }
                
        }

        public void Contains(Vector2f mouse)
        {
            if (box.GetGlobalBounds().Contains(mouse.X, mouse.Y))
            {
                Console.WriteLine("get input");
                TakeInput = true;
            }
            else
            {
                TakeInput = false;
            }
        }

        public void ClearText()
        {
            playerInput = "";
        }

        public string GetText()
        {
            var input = playerInput;
            playerInput = "";
            TakeInput = false;
            return input;
        }
    }
}
