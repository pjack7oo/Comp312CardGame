using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompCardGame.Source.Objects;
using CompCardGame.Source.Field;

namespace CompCardGame.Source.Core
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

        public int id;//to be used by database

        public List<Card> cards;//all cards owned by the player
        public List<List<Card>> decks;//player decks

        private Queue<Card> activeDeck;//deck will probably switch to stack which makes more sense for a deck
        List<Card> hand;
        List<Card> graveYard;

        //Selected card for handling when player grabs a card
        //Card selectedCard;
        //target for when selecting target to attack or use spell on
        //object target;

        private int health;
        public int Health { get { return health; } set { health = value; healthText.DisplayedString = $"Health: {value}"; } }


        //help with drawing location on the board
        public PlayerType PlayerType { get; private set; }
        private int crystals;
        public int Crystals { get { return crystals; } private set { crystals = value; crystalsText.DisplayedString = $"Crystals: {value}"; } }
        //this is for when a card increases the amount of crystals to be received for the turn
        public int CrystalsToAdd { get; set; }

        public int MaxCrystals { get; private set; }

        private Queue<Card> cardsToRemove = new Queue<Card>();

        private Text healthText;

        private Text crystalsText;

        public ViewType viewType;

        private RectangleShape graveyardOutline;
        private RectangleShape deckOutline;
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
            PlayerType = playerType;
            activeDeck = new Queue<Card>();
            hand = new List<Card>();
            graveYard = new List<Card>();
            Boolean temp = true;
            int tempI = 0;
            //temporary for loop for testing later wont be needed because it will load in the players cards
            for (int i = 0; i < 30; i++)
            {

                if (i % 2 == 0)
                {
                    if (tempI == 0)
                    {
                        var card = new SpellCard();

                        card.cardName.DisplayedString = $"{i}";
                        activeDeck.Enqueue(card);
                        tempI = 1;
                    }
                    else
                    {
                        var card = new SpellCard();
                        card.SetEffect(Effect.OverloadCardMana(2, card));
                        card.cardName.DisplayedString = $"Overload Mana";
                        activeDeck.Enqueue(card);
                        tempI = 0;
                    }

                }
                else
                {
                    if (i == 3)
                    {
                        var card = new EffectMonster() { MaxMana = 2 };
                        var effects = new Effect[2];
                        effects[0] = Effect.HealPlayer(5, card, 1,2);
                        effects[1] = Effect.OverloadCardMana(1, card, 2, 0, true);
                        
                        //card.SetEffect(Effect.HealPlayer(5, card), 2);
                        card.SetEffects(effects);
                        card.cardName.DisplayedString = $"{i}";

                        card.Attack = 110;

                        activeDeck.Enqueue(card);

                    }
                    else
                    {
                        var card = new MonsterCard();
                        card.cardName.DisplayedString = $"{i}";
                        if (playerType == PlayerType.Player && temp)//temporary for testing
                        {
                            card.Attack = 110;

                            temp = false;
                        }
                        activeDeck.Enqueue(card);
                    }


                }

            }
            //cards = cards.Shuffle();






            if (playerType == PlayerType.Player)
            {
                healthText = HelperFunctions.NewText($"Health: ", 15, new Vector2f { X = 80f, Y = Game.ScreenHeight - 140f }, Color.Red);
                crystalsText = HelperFunctions.NewText($"Crystals: ", 15, new Vector2f { X = 180f, Y = Game.ScreenHeight - 140f }, Color.Blue);
            }
            else
            {
                healthText = HelperFunctions.NewText($"Health: ", 15, new Vector2f { X = 80f, Y = 120f }, Color.Red);
                crystalsText = HelperFunctions.NewText($"Crystals: ", 15, new Vector2f { X = 180f, Y = 120f }, Color.Blue);
            }

            Health = 30;

            MaxCrystals = 4;
            Crystals = MaxCrystals;
            CrystalsToAdd = 0;

        }


        //position the decks on their field
        public void SetDeckPosition()
        {
            //will stager these in the future so it looks like the deck has height, will also need shadows
            // bottom field deck position
            if (PlayerType == PlayerType.Player)
            {

                Vector2f position = new Vector2f(Game.ScreenWidth - Card.width * 2 - 220, Game.ScreenHeight - Card.height - 170);
                graveyardOutline = new RectangleShape(new Vector2f(Card.width, Card.height)) { Position = position, OutlineColor = Color.Red, OutlineThickness = 2, FillColor = Color.Transparent };



                position = new Vector2f(Game.ScreenWidth - Card.width * 2 - 220, Game.ScreenHeight - 150);
                deckOutline = new RectangleShape(new Vector2f(Card.width, Card.height)) { Position = position };
                //for (int i = 0; i < cards.Count; i++)
                //{
                //    cards[i].Position = position;
                //}
                foreach (var card in activeDeck)
                {
                    card.Position = position;
                }
            }
            else //top field deck position
            {


                Vector2f position = new Vector2f(-20, Card.height / 2 + 10);

                graveyardOutline = new RectangleShape(new Vector2f(Card.width, Card.height)) { Position = position, OutlineColor = Color.Red, OutlineThickness = 2, FillColor = Color.Transparent };



                position = new Vector2f(-20, -170);
                deckOutline = new RectangleShape(new Vector2f(Card.width, Card.height)) { Position = position };
                //for (int i = 0; i < cards.Count; i++)
                //{
                //    cards[i].Position = position;
                //}
                foreach (var card in activeDeck)
                {
                    card.Position = position;
                }
            }
        }
        ////testing things with reseting nicely
        //public void ResetCards()
        //{
        //    for(int i = 0; i <hand.Count;i++)
        //    {
        //        hand[i].Position = new Vector2f(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height / 2 + 10);
        //    }
        //}
        ////testing things with reseting nicely
        //public void ResetCardPosition(Card card)
        //{
        //    for(int i = 0; i < hand.Count;i++)
        //    {
        //        if (hand[i] == card)
        //        {

        //            Console.WriteLine(true);
        //            card.Position = new Vector2f(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height / 2 + 10);

        //        }
        //    }
        //}

        public void SendCardToGraveyard(Card card)
        {
            graveYard.Add(card);
            if (PlayerType == PlayerType.Player)
            {
                Vector2f position = new Vector2f(Game.ScreenWidth - Card.width * 2 - 220, Game.ScreenHeight - Card.height - 170);

                card.Position = position;

            }
            else
            {
                Vector2f position = new Vector2f(-20, Card.height / 2 + 10);

                card.Position = position;

            }
        }

        public void AddCardToRemoveQueue(Card card)
        {
            cardsToRemove.Enqueue(card);
        }
        //to be called once per turn right after being given to player
        public void ResetCrystalsToAdd()
        {
            CrystalsToAdd = 0;
        }

        //get crystals during drawing phase and 
        public void GetCrystals()
        {
            Crystals = MaxCrystals;
            Crystals += CrystalsToAdd;
            ResetCrystalsToAdd();
        }
        //to be used when a card or spell grants crystals
        public void AddCrystals(int num)
        {

        }

        public void RemoveCrystals(int num)
        {
            Crystals -= num;
        }

        //as game progresses this will be used to increase amount of crystals the player can get from turn start
        public void IncreaseMaxCrystals(int num)
        {

        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            if (viewType == ViewType.FieldView)
            {
                target.Draw(deckOutline);
                target.Draw(graveyardOutline);
                DrawDeck(target, states);
                DrawHand(target, states);
                DrawGraveyard(target, states);
            }
            else
            {
                target.Draw(healthText);//temporary later will have a drawing and sprite 
                target.Draw(crystalsText);//^^^^^
            }


        }

        public void DrawGraveyard(RenderTarget target, RenderStates states)
        {
            foreach (var card in graveYard)
            {
                card.viewType = ViewType.FieldView;
                target.Draw(card, states);
            }
        }

        //drawing the cards in your hand
        private void DrawHand(RenderTarget target, RenderStates states)
        {
            foreach (var card in hand)
            {
                card.viewType = ViewType.FieldView;

                target.Draw(card);


            }

        }

        //drawing the deck of cards
        private void DrawDeck(RenderTarget target, RenderStates states)
        {
            foreach (var card in activeDeck)
            {
                card.viewType = ViewType.FieldView;
                target.Draw(card);
            }
        }
        //draw a card and add to hand
        public void DrawACardFromDeck()
        {
            if (hand.Count < MaxCardsInHand)
            {
                hand.Add(activeDeck.Dequeue());
                //cards.RemoveAt(0);
                SetPositionsOfHand();
            }
        }
        //remove card from hand this is called when moved elsewhere
        public void RemoveCard(Card card)
        {
            hand.Remove(card);
        }
        //when a card is drawn we will update the cards position, state, location
        private void SetPositionsOfHand()
        {
            if (PlayerType == PlayerType.Player)
            {
                Update();//make sure things are updated
                for (int i = 0; i < hand.Count; i++)
                {

                    hand[i].Location = CardLocation.Hand;
                    hand[i].State = CardState.Front;
                    hand[i].Position = hand[i].previousPosition = new Vector2f(i * (Card.width + 20) + 200, Game.ScreenHeight + Card.height / 2 + 20);
                    hand[i].UpdatePositions();
                    //target.Draw(CardOutlineRectangle(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height -160));
                }
            }
            else
            {
                for (int i = 0; i < hand.Count; i++)
                {
                    hand[i].Location = CardLocation.Hand;
                    hand[i].Position = new Vector2f(i * (Card.width + 20) + 200, 0 - Card.height * 1.5f - 20);
                    hand[i].UpdatePositions();
                    //target.Draw(CardOutlineRectangle(i * (Card.width + 20) + 410, Game.ScreenHeight - Card.height -160));
                }
            }
        }

        public Card GrabRandomCard()//might be used by somecard abilities and using it for testing opponent will stillo need AI
        {
            var random = new Random();
            if (hand.Count == 0)
            {
                return null;
            }
            var card = hand[random.Next(0, hand.Count)];
            foreach (var cardToBeRemoved in cardsToRemove)
            {
                if (card == cardToBeRemoved)
                {
                    foreach (var card2 in hand)
                    {
                        if (card != card2)
                        {
                            return card2;
                        }
                    }
                }
            }
            return card;
        }
        //check if click on cards in hand
        public Card HandleMouseClick(Vector2f mouse)
        {
            foreach (var card in hand)
            {

                if (card.Contains(mouse) && card.Location == CardLocation.Hand)
                {

                    return card;
                }
            }
            return null;
        }

        public Card HandleMouseClickForOppenentTurn(Vector2f mouse)
        {
            foreach (var card in hand)
            {

                if (card.Contains(mouse) && card.Location == CardLocation.Hand)
                {
                    //card.Location = CardLocation.Moving;
                    return card;
                }
            }
            return null;
        }

        //this will be updates that are independent of matchstate so for example removing cards from hand/graveyard
        public void Update()
        {

            while (cardsToRemove.Count > 0)
            {
                var card = cardsToRemove.Dequeue();
                RemoveCard(card);
            }

        }

        //handle when card is hovered over by player
        public void HandleMouseMovement(Vector2f mouse)
        {

            //checking if mouse moves over the cards in the hand
            for (int i = 0; i < hand.Count; i++)
            {
                //Console.WriteLine(hand[i].Position);
                if (hand[i].Contains(mouse))
                {
                    //var pos = new Vector2f(0, -30);

                    //lift card to view it
                    hand[i].LiftCardUp();

                }
                else
                {
                    if (hand[i].Selected)
                    {
                        //lower card when no longer hovering over it
                        hand[i].SetCardDown();
                    }
                    //Console.WriteLine(false);
                }
            }
            //if when mouse is clicked and moving a card
            //if (selectedCard != null)
            //{
            //needs to be shifted so mouse in middle of the card
            //    selectedCard.Position = mouse;
            //}
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
        public static Queue<T> Shuffle<T>(this Queue<T> queue)
        {
            List<T> list = queue.ToList();
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
            return new Queue<T>(list);
        }
    }

}
