using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace CompCardGame.Source.Core
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

        private readonly bool noSprite;//temporary later there will be a sprite for everything

        private System.TimeSpan pressedTime;

        private System.TimeSpan duration = TimeSpan.FromSeconds(1);
        private Sprite defaultSprite;
        private Sprite pressedSprite;
        private Sprite hoverSprite;

        private Text text;

        private Boolean active = false;

        public Boolean IsUsable { get; set; }
        //this is kind of a mess ima clean it later //TODO
        public Button(String text, uint charSize, Vector2f location, Color textColor, Action action, Vector2f scale)
        {
            this.text = new Text(text, HelperFunctions.font, charSize) { FillColor = textColor};

            noSprite = false;
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
            IsUsable = true;


            
            this.text.Position = HelperFunctions.GetCenteredPosition(new Vector2f(defaultSprite.GetGlobalBounds().Width, defaultSprite.GetGlobalBounds().Height), new Vector2f(this.text.GetGlobalBounds().Width, this.text.GetGlobalBounds().Height));

            ButtonState = ButtonState.Default;

            
        }

        public Button(String text, uint charSize, Vector2f location, Color textColor, Action action )
        {
            this.text = new Text(text, HelperFunctions.font, charSize) { FillColor = textColor };
            noSprite = false;

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

            IsUsable = true;


            this.text.Position = HelperFunctions.GetCenteredPosition(new Vector2f(defaultSprite.GetGlobalBounds().Width, defaultSprite.GetGlobalBounds().Height), new Vector2f(this.text.GetGlobalBounds().Width, this.text.GetGlobalBounds().Height));

            ButtonState = ButtonState.Default;


        }

        public Button(String text, uint charSize, Vector2f location, Shape shape, Action action)
        {
            noSprite = true;
            this.shape = shape;
            
            this.text = new Text(text, HelperFunctions.font) { FillColor = Color.Black};
            Origin = new Vector2f(this.shape.GetGlobalBounds().Width / 2, this.shape.GetGlobalBounds().Height / 2);
            Position = location;
            //shape = new RectangleShape(new Vector2f(width, height)) { FillColor = Color.Cyan};
            boundingBox = new RectangleShape(new Vector2f(this.shape.GetGlobalBounds().Width, this.shape.GetGlobalBounds().Height)) { FillColor = Color.Transparent, OutlineThickness = 3, OutlineColor = Color.Red };
            boundingBox.Origin = Origin;
            boundingBox.Position = Position;

            this.action = action;

            IsUsable = true;

            if (shape is RectangleShape)
            {
                this.text.Position = HelperFunctions.GetCenteredPosition(new Vector2f(this.shape.GetGlobalBounds().Width, this.shape.GetGlobalBounds().Height), new Vector2f(this.text.GetGlobalBounds().Width, this.text.GetGlobalBounds().Height));
            }
            else
            {
                this.text.Position = new Vector2f(Origin.X-12f, Origin.Y-19f);
            }
            

            ButtonState = ButtonState.Default;
        }

        public Boolean Contains(Vector2f mouse)
        {
            return boundingBox.GetGlobalBounds().Contains(mouse.X, mouse.Y);
        }


        
        public void Draw(RenderTarget target, RenderStates states)
        {
            
            if (IsUsable)
            {
                if (noSprite)
                {
                    states.Transform = Transform;
                    target.Draw(shape, states);
                    
                    target.Draw(text, states);
                    target.Draw(boundingBox);
                }
                else
                {
                    states.Transform = Transform;
                    switch (ButtonState)
                    {

                        case ButtonState.Default://draw sprite for default
                            target.Draw(defaultSprite, states);
                            target.Draw(text, states);
                            break;
                        case ButtonState.Hover://draw sprite for hover
                            target.Draw(defaultSprite, states);
                            target.Draw(text, states);
                            break;
                        case ButtonState.Pressed://draw sprite for pressed //or animation
                            target.Draw(pressedSprite, states);
                            target.Draw(text, states);

                            break;
                        default:
                            break;
                    }
                }



                
            }
            
            //draw image or shape

        }
        //update based on passed time
        public void Update(System.TimeSpan time)
        {
            if (IsUsable)
            {
                if (active)
                {
                    if (time - pressedTime >= TimeSpan.FromSeconds(1))
                    {
                        active = false;
                    }
                }
                if (ButtonState == ButtonState.Pressed)
                {
                    Console.WriteLine(time);
                    if (time - pressedTime >= duration)
                    {
                        ButtonState = ButtonState.Default;
                    }
                }
            }
            
        }

        public void EnableActionUse()
        {
            active = false;
        }

        //important any action passed must have its own checks
        //to make sure it can be used at that moment that way this just calls the action
        public void DoAction()
        {
           if (IsUsable)
            {
                if (!active)
                {
                    pressedTime = Game.GetTimeStamp();
                    ButtonState = ButtonState.Pressed;

                    action();
                    active = true;
                }

            }
            
            
            
        }
        

        
    }
}
