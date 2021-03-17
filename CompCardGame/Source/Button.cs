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
    public enum ButtonState
    {
        Default,
        Hover,
        Pressed
    }
    class Button: Transformable, Drawable
    {
        

        //might want a unique font 

        public ButtonState ButtonState { get; set; }

        private readonly Action action;//callback that button must execute when pressed
        private readonly Shape shape;
        private readonly Shape boundingBox;


        private System.TimeSpan pressedTime;

        private System.TimeSpan duration = TimeSpan.FromSeconds(1);
        private Sprite defaultSprite;
        private Sprite pressedSprite;
        private Sprite hoverSprite;

        private Text text;
        //this is kind of a mess ima clean it later //TODO
        public Button(String text, uint charSize, Vector2f location, int width, int height, Color textColor, Action action)
        {
            this.text = new Text(text, HelperFunctions.font, charSize) { FillColor = textColor};
            Position = location;
            shape = new RectangleShape(new Vector2f(width, height)) { FillColor = Color.Cyan};
            boundingBox = new RectangleShape(new Vector2f(width, height)) { FillColor = Color.Transparent, OutlineThickness = 1, OutlineColor = Color.Red };
           

            this.action = action;

            
            //load in the sprites
            defaultSprite = new Sprite(new Texture("Media/Images/GenericBtn.png"));

            pressedSprite = new Sprite(new Texture("Media/Images/GenericBtnPressed.png"));

            this.text.Position = GetCenteredPosition(new Vector2f(defaultSprite.GetGlobalBounds().Width, defaultSprite.GetGlobalBounds().Height), new Vector2f(this.text.GetGlobalBounds().Width, this.text.GetGlobalBounds().Height));

            ButtonState = ButtonState.Default;

            UpdatePositions();
        }

        public Button(String text, uint charSize, Vector2f location, Shape shape)
        {
            this.shape = shape;
            this.text = new Text(text, HelperFunctions.font);
            Position = location;
        }

        public Boolean Contains(Vector2f mouse)
        {
        
            return (boundingBox.GetGlobalBounds().Contains(mouse.X, mouse.Y))? true : false;
        }


        public void UpdatePositions()
        {
            boundingBox.Position = Position;
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform = Transform;
            
            switch(ButtonState)
            {
                case ButtonState.Default://draw sprite for default
                    target.Draw(defaultSprite, states);
                    break;
                case ButtonState.Hover://draw sprite for hover
                    target.Draw(defaultSprite, states);
                    break;
                case ButtonState.Pressed://draw sprite for pressed //or animation
                    target.Draw(pressedSprite, states);
                    
                    break;
                default:
                    break;
            }
            
            
            target.Draw(text, states);
            //draw image or shape

        }
        //update based on passed time
        public void Update(System.TimeSpan time)
        {
            if (ButtonState == ButtonState.Pressed)
            {
                Console.WriteLine(time);
                if (time - pressedTime >= duration)
                {
                    ButtonState = ButtonState.Default;
                }
            }
        }

        //important any action passed must have its own checks
        //to make sure it can be used at that moment that way this just calls the action
        public void DoAction()
        {
            pressedTime = Game.GetTimeStamp();
            ButtonState = ButtonState.Pressed;
            
            action();
            
        }
        //center the text
        private static Vector2f GetCenteredPosition(Vector2f buttonSize, Vector2f textSize)
        {
            Console.WriteLine(textSize); 
            int x = ((int)buttonSize.X >> 1) - ((int)textSize.X >> 1);//bitshift is faster than division only works for evens this case we divide by 2
            var y = buttonSize.Y / 2 - (textSize.Y );
            Vector2f result = new Vector2f(x,y);//TODO calculate center

            return result;
        }

        
    }
}
