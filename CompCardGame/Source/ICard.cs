using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{
    public enum CardState
    {
        Front,
        Back
    }

    public enum CardLocation
    {
        Deck,
        Hand,
        Field,
        Graveyard,
        Moving
    }
    interface ICard //might be needed
    {
        
        void Draw(RenderTarget target, RenderStates states);
    }
}
