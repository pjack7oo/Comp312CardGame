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
        public Button(String text, uint charSize, Vector2f location, Color textColor, Action action, Vector2f scale)
        {
            this.text = new Text(text, HelperFunctions.font, charSize) { FillColor = textColor};
           

            //load in the sprites
            defaultSprite = new Sprite(new Texture("Media/Images/GenericBtn.png"));

            pressedSprite = new Sprite(new Texture("Media/Images/GenericBtnPressed.png"));
            
            defaultSprite.Scale = scale;
            pressedSprite.Scale = scale;
            Origin = new Vector2f(defaultSprite.GetGlobalBounds().Width / 2, defaultSprite.GetGlobalBounds().Height / 2);
            Position = location;
            //shape = new RectangleShape(new Vector2f(width, height)) { FillColor = Color.Cyan};
            boundingBox = new RectangleShape(new Vector2f(defaultSprite.GetGlobalBounds().Width, defaultSprite.GetGlobalBounds().Height)) { FillColor = Color.Transparent, OutlineThickness = 1, OutlineColor = Color.Red };
            boundingBox.Origin = Origin;
            boundingBox.Position = Position;

            this.action = action;



            
            this.text.Position = HelperFunctions.GetCenteredPosition(new Vector2f(defaultSprite.GetGlobalBounds().Width, defaultSprite.GetGlobalBounds().Height), new Vector2f(this.text.GetGlobalBounds().Width, this.text.GetGlobalBounds().Height));

            ButtonState = ButtonState.Default;

            
        }

        public Button(String text, uint charSize, Vector2f location, Color textColor, Action action )
        {
            this.text = new Text(text, HelperFunctions.font, charSize) { FillColor = textColor };
            

            //load in the sprites
            defaultSprite = new Sprite(new Texture("Media/Images/GenericBtn.png"));

            pressedSprite = new Sprite(new Texture("Media/Images/GenericBtnPressed.png"));

            Origin = new Vector2f(defaultSprite.GetGlobalBounds().Width / 2, defaultSprite.GetGlobalBounds().Height / 2);
            Position = location;
            //shape = new RectangleShape(new Vector2f(width, height)) { FillColor = Color.Cyan};
            boundingBox = new RectangleShape(new Vector2f(defaultSprite.GetGlobalBounds().Width, defaultSprite.GetGlobalBounds().Height)) { FillColor = Color.Transparent, OutlineThickness = 1, OutlineColor = Color.Red };
            boundingBox.Origin = Origin;
            boundingBox.Position = Position;

            this.action = action;




            this.text.Position = HelperFunctions.GetCenteredPosition(new Vector2f(defaultSprite.GetGlobalBounds().Width, defaultSprite.GetGlobalBounds().Height), new Vector2f(this.text.GetGlobalBounds().Width, this.text.GetGlobalBounds().Height));

            ButtonState = ButtonState.Default;


        }

        public Button(String text, uint charSize, Vector2f location, Shape shape)
        {
            this.shape = shape;
            this.text = new Text(text, HelperFunctions.font);
            Position = location;
        }

        public Boolean Contains(Vector2f mouse)
        {
            return boundingBox.GetGlobalBounds().Contains(mouse.X, mouse.Y)? true : false;
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
        

        
    }
}
