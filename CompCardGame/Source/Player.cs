using System;
using System.Collections.Generic;
using SFML.Graphics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{
    class Player : Drawable
    {
        List<Card> cards;
        List<Card> hand;
        int health;

        public Player()
        {
            cards = new List<Card>();
            //temporary for loop for testing later wont be needed
            for (int i = 0; i < 30; i++)
            {
                cards.Add(new Card());
            }
            cards.shuffle();
        }
        
        public void Draw(RenderTarget target, RenderStates states)
        {
            drawHand(target, states);
        }

        private void drawHand(RenderTarget target, RenderStates states)
        {
            foreach (var card in cards)
            {
                target.Draw(card);
            }

        }//todo

        private void drawDeck() { }//todo
    }

    static class Extensions
    {
        public static void shuffle<T>(this List<T> list)
        {
            Random randomNum = new Random();
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int j = randomNum.Next(n + 1);
                T value = list[j];
                list[j] = list[n];
                list[n] = value;
            }
        }
    }

}
