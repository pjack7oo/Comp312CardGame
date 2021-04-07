using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;
using CompCardGame.Source.Field;

namespace CompCardGame.Source.Objects
{
    class Deck: Drawable
    {
        public Queue<Card> cards;
        private int maxCardCount = 30;
        private bool inDeckEditor = false;
        public List<Card> savedCards;

        public int ID { get; private set; }
        public Deck()
        {
            var random = new Random();
            savedCards = new List<Card>();
            cards = new Queue<Card>(savedCards);
            ID = random.Next();
        }
        //to be used by card manager
        public void AddCardToSavedDeck(Card card)
        {
            if (savedCards.Count<maxCardCount)
            {
                savedCards.Add(card);
            }
        }

        //to be used in match
        public void AddCardToDeck(Card card)
        {
            cards.Append(card);
        }
        public void SetDeckPosition(Vector2f position)
        {
            foreach (var card in cards)
            {
                card.Position = position;
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (!inDeckEditor)
            {
                foreach (var card in cards)
                {
                    card.viewType = ViewType.FieldView;
                    target.Draw(card);
                }
            }
            
        }
    }
}
