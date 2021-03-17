using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace CompCardGame.Source
{
    class HelperFunctions
    {

        //font used by card might be moved to helper class containing stuff like fonts and maybe loading in graphics
        public static Font font = new Font("Media/ARIALBD.TTF");

        //quicker way of creating text
        public static Text NewText(String text, uint charSize, Vector2f position, Color fillColor)
        {
            return new Text() { DisplayedString = text, Font = font, CharacterSize = charSize, FillColor = fillColor, Position = position };
        }

    }
}
