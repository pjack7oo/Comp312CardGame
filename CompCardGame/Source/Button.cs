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
    
    class Button: Transformable, Drawable
    {
        private enum ButtonState
        {
            Inactive,
            Active,
            Pressed
        }

        //might want a unique font 

        private ButtonState buttonState;

        private Shape shape;

        private Text text;
        public Button(String text, uint charSize, Vector2f location, int width, int height, Color textColor)
        {
            this.text = new Text(text, Card.font, charSize) { FillColor = textColor};
            Position = location;
            shape = new RectangleShape(new Vector2f(width, height)) { FillColor = Color.Cyan};

            this.text.Position = GetCenteredPosition(location, new Vector2f(this.text.GetGlobalBounds().Width, this.text.GetGlobalBounds().Height));
            buttonState = ButtonState.Inactive;
        }

        public Button(String text, uint charSize, Vector2f location, Shape shape)
        {
            this.shape = shape;
            this.text = new Text(text, Card.font);
            Position = location;
        }

        public Boolean Contains(Vector2f mouse)
        {
            return (shape.GetGlobalBounds().Contains(mouse.X, mouse.Y))? true : false;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform = Transform;
            
            switch(buttonState)
            {
                case ButtonState.Inactive:
                    break;
                case ButtonState.Active:
                    break;
                case ButtonState.Pressed:
                    break;
                default:
                    break;
            }
            target.Draw(shape,states);
            
            target.Draw(text, states);
            //draw image or shape

        }


        private static Vector2f GetCenteredPosition(Vector2f position, Vector2f textSize)
        {
            Vector2f result = new Vector2f();//TODO calculate center

            return result;
        }
    }
}
