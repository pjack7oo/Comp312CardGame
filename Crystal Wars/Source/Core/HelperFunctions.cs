using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace Crystal_Wars.Source.Core
{
    class HelperFunctions
    {
        public static Random random = new Random();
        //font used by card might be moved to helper class containing stuff like fonts and maybe loading in graphics
        public static Font font = new Font("Media/ARIALBD.TTF");

        //quicker way of creating text
        public static Text NewText(String text, uint charSize, Vector2f position, Color fillColor)
        {
            return new Text() { DisplayedString = text, Font = font, CharacterSize = charSize, FillColor = fillColor, Position = position };
        }

        //center the text
        public static Vector2f GetCenteredPosition(Vector2f buttonSize, Vector2f textSize)
        {
            //Console.WriteLine(textSize);
            int x = ((int)buttonSize.X >> 1) - ((int)textSize.X >> 1);//bitshift is faster than division only works for evens this case we divide by 2
            var y = buttonSize.Y / 2 - (textSize.Y);
            Vector2f result = new Vector2f(x, y);//TODO calculate center

            return result;
        }

    }
}
