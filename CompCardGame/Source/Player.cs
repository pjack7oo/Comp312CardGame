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
        //to make sure player never has more than 5 cards
        public static int MaxCardsInHand = 5;
        
        List<Card> cards;//deck
        List<Card> hand;
        List<Card> graveYard;
        public int Health { get; private set; }
        //help with drawing location on the board
        private PlayerType playerType;

        
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
            DrawHand(target, states);
            DrawDeck(target, states);
        }
        //drawing the cards in your hand
        private void DrawHand(RenderTarget target, RenderStates states)
        {
            foreach (var card in hand)
            {
                target.Draw(card);
            }

        }

        //drawing the deck of cards
        private void DrawDeck(RenderTarget target, RenderStates states) 
        {
            foreach (var card in cards)
            {
                target.Draw(card);
            }
        }
        //draw a card and add to hand
        public void DrawACardFromDeck()
        {
            if (hand.Count < MaxCardsInHand)
            {
                hand.Add(cards[0]);
                cards.RemoveAt(0);
                setPositionsOfHand();
            }
        }
        //when a card is drawn we will update the cards position, state, location
        private void setPositionsOfHand()
        {
            if (playerType == PlayerType.Player)
            {
                for(int i = 0; i < hand.Count;i++)
                {
                    hand[i].Location = CardLocation.Hand;
                    hand[i].State = CardState.Front;
                    hand[i].Position = new Vector2f(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height/2 +10);
                    hand[i].updatePositions();
                    //target.Draw(CardOutlineRectangle(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height -160));
                }
            }
            else
            {
                for (int i = 0; i < hand.Count; i++)
                {
                    hand[i].Location = CardLocation.Hand;
                    hand[i].Position = new Vector2f(i * (Card.width + 20) + 410, 0-Card.height/2-10);
                    hand[i].updatePositions();
                    //target.Draw(CardOutlineRectangle(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height -160));
                }
            }
        }

        
        
        //handle when card is hovered over by player
        public void HandleMouseMovement(Vector2f mouse)
        {
            
            //checking if mouse moves over the cards in the hand
            for(int i = 0; i < hand.Count;i++)
            {
                
                if (hand[i].contains(mouse))
                {
                    var pos = new Vector2f(0, -30);
                    //Console.WriteLine(true);
                    //lift card to view it
                    hand[i].liftCardUp();
                    
                } 
                else
                {
                    if (hand[i].Active)
                    {
                        //lower card when no longer hovering over it
                        hand[i].setCardDown();
                    }
                    //Console.WriteLine(false);
                }
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
