using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{
    public enum PlayerType
    {
        Player,
        Enemy
    }
    class Player : Drawable
    {
        List<Card> cards;//deck
        List<Card> hand;
        List<Card> graveYard;
        public int Health { get; private set; }
        //help with drawing location on the board
        private PlayerType playerType;

        ////une
        //public Player()
        //{
        //    type = PlayerType.Player;
        //    cards = new List<Card>();

        //    //temporary for loop for testing later wont be needed
        //    for (int i = 0; i < 30; i++)
        //    {
        //        cards.Add(new Card());
        //    }
        //    cards.shuffle();
        //    Health = 30;
        //}

        //will be called every time match is started
        public Player(PlayerType playerType)
        {
            this.playerType = playerType;
            cards = new List<Card>();
            hand = new List<Card>();
            graveYard = new List<Card>();
            //temporary for loop for testing later wont be needed because it will load in the players cards
            for (int i = 0; i < 30; i++)
            {
                cards.Add(new Card());
            }
            cards.shuffle();
            Health = 30;
        }

        //position the decks on their field
        public void setDeckPosition()
        {
            //will stager these in the future so it looks like the deck has height, will also need shadows
            // bottom field deck position
            if (playerType == PlayerType.Player) 
            {
                Vector2f position = new Vector2f(Game.ScreenWidth - Card.width - 190-20, Game.ScreenHeight - Card.height - 160);
                for (int i = 0; i < cards.Count; i++)
                {
                    cards[i].Position = position;
                }
            }
            else //top field deck position
            {
                Vector2f position = new Vector2f(190,  160);
                for (int i = 0; i < cards.Count; i++)
                {
                    cards[i].Position = position;
                }
            }
        }
        
        public void Draw(RenderTarget target, RenderStates states)
        {
            drawHand(target, states);
            drawDeck(target, states);
        }
        //drawing the cards in your hand
        private void drawHand(RenderTarget target, RenderStates states)
        {
            foreach (var card in hand)
            {
                target.Draw(card);
            }

        }

        //drawing the deck of cards
        private void drawDeck(RenderTarget target, RenderStates states) 
        {
            foreach (var card in cards)
            {
                target.Draw(card);
            }
        }

        //when card hits player
        public void ApplyDamage(int damage)
        {
            Health -= damage;
        }
    }

    static class Extensions
    {
        //Fisher-Yates shuffle
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
