using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace Crystal_Wars.Source.Core
{
    class InputHandler:Drawable //TODO hold all the functions for movement and control the buttons 
    {
        private Match match;

        public List<Button> buttons;

        private RenderWindow window;

        public InputHandler(RenderWindow window)
        {
            this.window = window;
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

        public void OnKeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Space)
            {
                match.ClearCardSelection();
            }
        }


        //handles mouseMovement
        public void MouseMovement(object sender, MouseMoveEventArgs e)
        {
            var mouse = new Vector2i(e.X, e.Y);
            Vector2f worldPos;
            //might need this to change based on state
            switch (Game.GameState)
            {
                case GameState.Loading:

                    break;
                case GameState.MainPage://TODO check if hovering over button
                    worldPos = window.MapPixelToCoords(mouse, Game.defaultView);
                    CheckButtonHover(worldPos);
                    break;
                
                case GameState.Match:
                    worldPos = window.MapPixelToCoords(mouse, Game.fieldView);
                    match.MouseMovement(worldPos);
                    CheckButtonHover(worldPos);
                    break;
                case GameState.Settings:
                    break;
                case GameState.CardManager:
                    Game.cardManager.MouseMovement(mouse);
                    break;
                case GameState.Online:
                    worldPos = window.MapPixelToCoords(mouse, Game.fieldView);
                    match.MouseMovement(worldPos);
                    CheckButtonHover(worldPos);
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
            for(int i = 0; i <buttons.Count; i++)
            {
                if (buttons[i] != null)
                {
                    if (buttons[i].Contains(mouse))
                    {

                        buttons[i].DoAction();
                    }
                }
            }
        }

        public void MouseClick(object sender, MouseButtonEventArgs e)//this might need the state of the turn in future
        {
            if (e.Button == Mouse.Button.Left)
            {
                //TODO add handler for player1
                //Console.WriteLine($"{e.X}, {e.Y}");
                var mouse = Mouse.GetPosition(window);
                Vector2f worldPos;

                switch (Game.GameState)
                {
                    case GameState.MainPage://TODO check if click on a button
                        worldPos = window.MapPixelToCoords(mouse, Game.defaultView);
                        CheckButtonClick(new Vector2f(e.X, e.Y));
                        break;
                    case GameState.Match:
                        worldPos = window.MapPixelToCoords(mouse, Game.fieldView);
                        CheckButtonClick(worldPos);
                        match.MouseClick(worldPos);
                        break;
                    case GameState.CardManager:
                        //var mouse2 = new Vector2f(e.X, e.Y);
                        Game.cardManager.MouseClick(mouse);
                        break;
                    case GameState.Online:
                        worldPos = window.MapPixelToCoords(mouse, Game.defaultView);
                        CheckButtonClick(worldPos);
                        match.MouseClick(worldPos);
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
            var mouse = Mouse.GetPosition(window);
            Vector2f worldPos = window.MapPixelToCoords(mouse);
            switch (Game.GameState)
            {
                case GameState.MainPage://TODO check 
                    worldPos = window.MapPixelToCoords(mouse, Game.defaultView);
                    break;
                case GameState.Match:
                    worldPos = window.MapPixelToCoords(mouse, Game.fieldView);
                    match.MouseReleased(worldPos);
                    break;
                case GameState.CardManager:
                    Game.cardManager.MouseReleased(mouse);
                    break;
                case GameState.Online:
                    worldPos = window.MapPixelToCoords(mouse, Game.fieldView);
                    match.MouseReleased(worldPos);
                    break;
                default:
                    break;
            }
            


        }


        public void ScrollWheel(object sender, MouseWheelScrollEventArgs e)
        {

            switch (Game.GameState)
            {
                case GameState.CardManager:
                    Game.cardManager.ScrollWheel(e);
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
