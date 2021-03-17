using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace CompCardGame.Source
{
    class InputHandler:Drawable //TODO hold all the functions for movement and control the buttons 
    {
        private Match match;

        public List<Button> buttons;


        public InputHandler()
        {
            buttons = new List<Button>();
        }

        public void SetMatch(Match match)
        {
            this.match = match;
        }

        public void AddButton(Button button)//TODO add button
        {
            buttons.Add(button);
        }

        public void AddButtons(IEnumerable<Button> buttons)
        {
            this.buttons.AddRange(buttons);
        }

        public void ClearButtons()//TODO clear buttons
        {
            buttons.Clear();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach(var button in buttons)
            {
                target.Draw(button);
            }
        }

        //handles mouseMovement
        public void MouseMovement(object sender, MouseMoveEventArgs e)
        {
            var mouse = new SFML.System.Vector2f(e.X, e.Y);
            //might need this to change based on state
            switch (Game.GameState)
            {
                case GameState.Loading:

                    break;
                case GameState.MainPage://TODO check if hovering over button
                    CheckButtonHover(mouse);
                    break;
                case GameState.Match:
                    match.MouseMovement(mouse);
                    CheckButtonHover(mouse);
                    break;
                case GameState.Settings:
                    break;
                default:
                    break;
            }

        }

        public void CheckButtonHover(Vector2f mouse)
        {
            foreach(var button in buttons)
            {
                if (button.Contains(mouse) && button.ButtonState == ButtonState.Default ){
                    button.ButtonState = ButtonState.Hover;
                } else
                {
                    if (button.ButtonState == ButtonState.Hover)
                    {
                        button.ButtonState = ButtonState.Default;
                    }
                }
            }
        }

        public void CheckButtonClick(Vector2f mouse)
        {
            foreach (var button in buttons)
            {
                if (button.Contains(mouse))
                {
                    
                    button.DoAction();
                }
                
            }
        }

        public void MouseClick(object sender, MouseButtonEventArgs e)//this might need the state of the turn in future
        {
            if (e.Button == Mouse.Button.Left)
            {
                //TODO add handler for player1
                //Console.WriteLine($"{e.X}, {e.Y}");
                var mouse = new SFML.System.Vector2f(e.X, e.Y);


                switch (Game.GameState)
                {
                    case GameState.MainPage://TODO check if click on a button
                        CheckButtonClick(mouse);
                        break;
                    case GameState.Match:
                        CheckButtonClick(mouse);
                        match.MouseClick(mouse);
                        break;
                    default:
                        break;
                }


            }
            else if (e.Button == Mouse.Button.Right)//temporary for testing
            {
                Match.TurnState = TurnState.Attack;
            }
        }


        //this will be important for handling when card is placed on field and when attacking opponent
        public void MouseReleased(object sender, MouseButtonEventArgs e)
        {
            var mouse = new SFML.System.Vector2f(e.X, e.Y);
            switch (Game.GameState)
            {
                case GameState.MainPage://TODO check 
                    break;
                case GameState.Match:
                    match.MouseReleased(mouse);
                    break;
                default:
                    break;
            }
            


        }
        //update buttons
        public void Update(System.TimeSpan time)
        {
            
            foreach(var button in buttons)
            {
                button.Update(time);
            }
        }
    }
}
