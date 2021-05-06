using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystal_Wars.Source.Objects;
using MongoDB.Bson;

namespace Crystal_Wars.Source.Core
{
    class CardManager
    {

        private enum ManagerState
        {
            DeckSelection,
            DeckEdit
        }

        private enum Location
        {
            Deck,
            Cards
        }
        private RenderWindow window;

        private Text title;
        private List<Text> deckEditText = new List<Text>();
        private List<Text> deckSelectText = new List<Text>();
        private Player player;
        private View Cardsview;
        private View CardView;
        private View DeckEditView;
        private View deckView;
        private List<Shape> shapes;
        private Deck activeDeck;

        private List<Button> deckSelectButtons;
        private List<Button> deckEditButtons;

        private List<Card> cards;
        private ManagerState managerState = ManagerState.DeckSelection;

        private Card selectedCard;
        private Card lastSelectedCard;
        private float maxScrollLength;

        private Vector2f cardsViewCenter;

        private RectangleShape deckBoundingBox;
        private RectangleShape cardsBoundingBox;

        private Location? startingLocation;

        public CardManager(RenderWindow renderWindow, Player player)
        {
            window = renderWindow;
            title = HelperFunctions.NewText("Card and Deck Manager", 40, new Vector2f(Game.ScreenWidth / 2, 20), new Color(101, 67, 33));
            title.Origin = new Vector2f(title.GetGlobalBounds().Width / 2, title.GetGlobalBounds().Height / 2);
            this.player = player;

            Cardsview = new View(new FloatRect(0f, 0f, Game.ScreenWidth * 0.4f, Game.ScreenHeight));
            Cardsview.Zoom(2f);
            Cardsview.Viewport = new FloatRect(0.6f, 0.08f, 0.4f, 1f);
            Cardsview.Move(new Vector2f(Game.ScreenWidth * 0.4f / 2 - Card.width, Game.ScreenHeight / 2 - Card.height/2-20));
            cardsViewCenter = Cardsview.Center;

            deckView = new View(new FloatRect(0f, 0f, Game.ScreenWidth*0.4f, Game.ScreenHeight));
            deckView.Zoom(2f);
            deckView.Viewport = new FloatRect(0.2f, 0.08f, 0.4f, 1f);
            deckView.Move(new Vector2f(Game.ScreenWidth * 0.4f / 2 - Card.width, Game.ScreenHeight / 2 - Card.height/2 -20));

            deckBoundingBox = new RectangleShape(new Vector2f(Game.ScreenWidth * 0.4f - 1, Game.ScreenHeight-1)) { OutlineThickness = 2, OutlineColor = Color.Green, Position = new Vector2f(Game.ScreenWidth*0.2f, 1), FillColor = Color.Transparent };
            cardsBoundingBox = new RectangleShape(new Vector2f(Game.ScreenWidth * 0.4f - 1, Game.ScreenHeight-1)) { OutlineThickness = 2, OutlineColor = Color.Red, Position = new Vector2f(Game.ScreenWidth * 0.6f, 1), FillColor = Color.Transparent };
            CardView = new View(new FloatRect(0f, 0f, Game.ScreenWidth / 4.8f, Game.ScreenHeight));
            CardView.Zoom(0.8f);
            CardView.Viewport = new FloatRect(0f, 0f, 0.2f, 1f);

            deckSelectButtons = new List<Button>();
            deckEditButtons = new List<Button>();

            DeckEditView = new View(new FloatRect(0f, 0f, Game.ScreenWidth * 0.8f, Game.ScreenHeight));
            DeckEditView.Viewport = new FloatRect(0.2f, 0f, 0.8f, 1f);



            InitiallizeShapes();
            InitiallizeDeckEditText();
            InitiallizeDeckSelectText();
            InitiallizeButtons();
            player.cards = new List<Card>();
            //Temporary 

            int tempI = 0;
            //temporary for loop for testing later wont be needed because it will load in the players cards
            for (int i = 0; i < 61; i++)
            {
                if (i % 4 == 0)
                {
                    if (tempI == 0)
                    {
                        var card = new SpellCard(ObjectId.GenerateNewId().ToString());
                        card.cardName.DisplayedString = $"{i}";
                        card.State = CardState.Front;
                        player.cards.Add(card);
                        tempI = 1;
                        Database.AddCardToPlayerCards(player, card);
                    }
                    else
                    {
                        var card = new SpellCard(ObjectId.GenerateNewId().ToString());
                        card.SetEffect(Effect.OverloadCardMana(2, card));
                        card.cardName.DisplayedString = $"Overload Mana";
                        card.State = CardState.Front;
                        player.cards.Add(card);
                        tempI = 0;
                        Database.AddCardToPlayerCards(player, card);
                    }
                }
                else
                {
                    if (i % 3 == 0)
                    {
                        var card = new EffectMonster(ObjectId.GenerateNewId().ToString()) { MaxMana = 2 };
                        var effects = new Effect[2];
                        effects[0] = Effect.HealPlayer(5, card, 1, 2);
                        effects[1] = Effect.OverloadCardMana(1, card, 2, 0, true);
                        //card.SetEffect(Effect.HealPlayer(5, card), 2);
                        card.SetEffects(effects);
                        card.cardName.DisplayedString = $"{i}";
                        card.State = CardState.Front;
                        card.Attack = 110;
                        player.cards.Add(card);
                        Database.AddCardToPlayerCards(player, card);
                    }
                    else
                    {
                        var card = new MonsterCard(ObjectId.GenerateNewId().ToString());
                        card.cardName.DisplayedString = $"{i}";
                        card.State = CardState.Front;
                        card.Attack = 110;
                        player.cards.Add(card);
                        Database.AddCardToPlayerCards(player, card);
                    }
                }
            }
            //not temp
            cards = player.CopyCards();
            OrganizeCards();
            SetCardPositions();
            
        }


        public void OnResize(SizeEventArgs e)
        {
            Cardsview = new View(new FloatRect(0f, 0f, e.Width * 0.4f, e.Height));
            Cardsview.Zoom(2f);
            Cardsview.Viewport = new FloatRect(0.6f, 0.08f, 0.4f, 1f);
            Cardsview.Move(new Vector2f(e.Width * 0.4f / 2 - Card.width, e.Height / 2 - Card.height));
            cardsViewCenter = Cardsview.Center;

            deckView = new View(new FloatRect(0f, 0f, e.Width * 0.4f, e.Height));
            deckView.Zoom(2f);
            deckView.Viewport = new FloatRect(0.2f, 0.08f, 0.4f, 1f);
            deckView.Center = new Vector2f(0, 0);

            deckBoundingBox = new RectangleShape(new Vector2f(e.Width * 0.4f-2, e.Height)) { OutlineThickness = 2, OutlineColor = Color.Green, Position = new Vector2f(e.Width * 0.2f, 2), FillColor = Color.Transparent };
            cardsBoundingBox = new RectangleShape(new Vector2f(e.Width * 0.4f-2, e.Height)) { OutlineThickness = 2, OutlineColor = Color.Red, Position = new Vector2f(e.Width * 0.6f, 2), FillColor = Color.Transparent };
            CardView = new View(new FloatRect(0f, 0f, e.Width / 4.8f, e.Height));
            CardView.Zoom(0.8f);
            CardView.Viewport = new FloatRect(0f, 0f, 0.2f, 1f);
        }

        //algorithm to organize cards 
        private void OrganizeCards()
        {

        }

        private void SetAvailableCards()
        {
            var cardsToRemove = new List<Card>();
            cards.Clear();
            cards = player.CopyCards();
            for(int i = 0; i < cards.Count;i++)
            {
                var card = cards[i];
                for (int j = 0; j <activeDeck.savedCards.Count;j++)
                {
                    var savedCard = activeDeck.savedCards[j];
                    
                    if (savedCard.Equals(card))
                    {
                        //Console.WriteLine($"true{card.ingameID}, {savedCard.ingameID}");
                        cardsToRemove.Add(card);
                    }
                }
            }
            foreach(var card in cardsToRemove)
            {
                cards.Remove(card);
            }

            SetCardPositions();
        }



        public void AddButton(Button button)
        {
            deckSelectButtons.Add(button);
        }

        private void SetCardPositions()
        {
            var length = cards.Count;
            //var cards = player.cards;
            var y = 0;
            var x = 0;
            for (int i = 0; i < length; i++)
            {
                if (i % 6 == 0 && i != 0)
                {
                    y++;
                    x = 0;
                }
                
                cards[i].Origin = new Vector2f(Card.width / 2, Card.height / 2);
                cards[i].Position = new Vector2f(x * (Card.width + 20), y * (Card.height + 20));
                cards[i].boundingBox.Origin = cards[i].Origin;
                cards[i].boundingBox.Position = cards[i].Position;

                cards[i].previousPosition = cards[i].Position;
                x++;


            }
            maxScrollLength = y * (Card.height + 20);
        }

        private void SetDeckCardPositions()
        {
            var length = activeDeck.savedCards.Count;
            //var cards = player.cards;
            var y = 0;
            var x = 0;
            for (int i = 0; i < length; i++)
            {
                if (i % 6 == 0 && i != 0)
                {
                    y++;
                    x = 0;
                }

                activeDeck.savedCards[i].Origin = new Vector2f(Card.width / 2, Card.height / 2);
                activeDeck.savedCards[i].Position = new Vector2f(x * (Card.width + 20), y * (Card.height + 20));
                activeDeck.savedCards[i].boundingBox.Origin = activeDeck.savedCards[i].Origin;
                activeDeck.savedCards[i].boundingBox.Position = activeDeck.savedCards[i].Position;

                activeDeck.savedCards[i].previousPosition = activeDeck.savedCards[i].Position;
                x++;


            }
        }


        private void ResetViewPos()
        {
            Cardsview.Center = cardsViewCenter;
        }

        private void CreateDeck()
        {
            var deck = new Deck();
            Database.CreateDeck(player, deck);
            player.decks.Add(deck);
            CreateDeckButton(deck);
            
        }

        private void CreateDeckButton(Deck deck)
        {
            var button = new Button("", 0, new Vector2f((player.decks.Count - 1) * (Card.width + 20) + 150, 400), Color.Black, new RectangleShape(new Vector2f(Card.width, Card.height)) { FillColor = new Color(139, 69, 10), OutlineColor = new Color(169, 169, 169), OutlineThickness = 2 }, () => { activeDeck = deck; ResetViewPos(); managerState = ManagerState.DeckEdit; SetAvailableCards(); SetDeckCardPositions(); });
            deckSelectButtons.Add(button);
        }

        public void MouseClick(Vector2i mouse)
        {
            var pos = window.MapPixelToCoords(mouse, Game.defaultView);
            if (managerState == ManagerState.DeckSelection)
            {
                for (int i = 0; i < deckSelectButtons.Count; i++)
                {
                    if (deckSelectButtons[i].Contains(pos))
                    {
                        deckSelectButtons[i].EnableActionUse();
                        deckSelectButtons[i].DoAction();
                    }
                }
            }
            else
            {
                for (int i = 0; i < deckEditButtons.Count; i++)
                {
                    if (deckEditButtons[i].Contains(pos))
                    {
                        deckEditButtons[i].EnableActionUse();
                        deckEditButtons[i].DoAction();
                    }
                }
                //var selectedCard2 = players[0].HandleMouseClickForOppenentTurn(mouse);
                //if (selectedCard2 != null)
                //{
                //    lastSelectedCard = selectedCard2;
                //}
                selectedCard = HandleMouseClick(mouse);
                startingLocation = GetTarget(mouse);
                if (selectedCard != null)
                {
                    
                    pos = window.MapPixelToCoords(mouse, DeckEditView);
                    selectedCard.UpdatePositions(pos);
                    lastSelectedCard = selectedCard;


                    selectedCard.Location = CardLocation.Moving;




                }
            }
        }

        public void ScrollWheel(MouseWheelScrollEventArgs mouseWheel)
        {
            Console.WriteLine(mouseWheel.Delta);
            var pos = window.MapPixelToCoords(new Vector2i(mouseWheel.X,mouseWheel.Y),Cardsview);

            if (mouseWheel.Delta * -1 > 0)
            {
                if (Cardsview.Center.Y + Game.ScreenHeight / 2<= maxScrollLength)
                {
                    Cardsview.Move(new Vector2f(0, -2 * mouseWheel.Delta * 10));
                }
            }
            else
            {
                if (Cardsview.Center.Y - Game.ScreenHeight / 2 -Card.height-50 >= 0)
                {
                    Cardsview.Move(new Vector2f(0, -2 * mouseWheel.Delta * 10));
                }
            }

            
            
            
            
        }

        public Card HandleMouseClick(Vector2i mouse)
        {

            var pos = window.MapPixelToCoords(mouse, Cardsview);
            //Console.WriteLine(pos);
            foreach (var card in cards)
            {

                if (card.Contains(pos))
                {

                    return card;
                }
            }

            pos = window.MapPixelToCoords(mouse, deckView);
            var deckCard = activeDeck.CheckMouseClick(pos);
            if (deckCard != null)
            {
                return deckCard;
            }
            return null;
        }

        //public Card HandleMouseClickForOppenentTurn(Vector2f mouse)
        //{
        //    foreach (var card in hand)
        //    {

        //        if (card.Contains(mouse) && card.Location == CardLocation.Hand)
        //        {
        //            //card.Location = CardLocation.Moving;
        //            return card;
        //        }
        //    }
        //    return null;
        //}

        public void MouseMovement(Vector2i mouse)
        {
            //players[0].HandleMouseMovement(mouse);

            if (selectedCard != null && selectedCard.Location == CardLocation.Moving)
            {
                //needs to be shifted so mouse in middle of the card
                var pos = window.MapPixelToCoords(mouse, DeckEditView);
                
                selectedCard.UpdatePositions(pos);
            }



        }

        public void MouseReleased(Vector2i mouse)
        {
            var pos = window.MapPixelToCoords(mouse, Cardsview);
            Location? target = GetTarget(mouse);
            Console.WriteLine($"{startingLocation }, { target}");
            if (startingLocation == Location.Cards)
            {
                if (selectedCard != null && target != Location.Deck) //check where to place card when mouse is released and if no valid slot is selected then drop the card back to previous spot
                {
                    if (selectedCard.Location == CardLocation.Moving)
                    {
                        selectedCard.Selected = false;
                        selectedCard.Location = CardLocation.Hand;
                        //player1.ResetCardPosition(selectedCard);
                        selectedCard.ResetCard();
                        selectedCard = null;
                        //Console.WriteLine("released");
                        startingLocation = null;
                        
                        //player1.ResetCards();
                    }

                }
                else
                {
                    if (selectedCard != null && target == Location.Deck)
                    {
                        var added = activeDeck.AddCardToSavedDeck(selectedCard);
                        if (added)
                        {
                            cards.Remove(selectedCard);
                            selectedCard.Selected = false;
                            selectedCard.Location = CardLocation.Hand;
                            Database.InsertCardIntoDeck(player, activeDeck, selectedCard);
                            selectedCard = null;
                            SetCardPositions();
                            SetDeckCardPositions();
                        }
                        else
                        {
                            selectedCard.Selected = false;
                            selectedCard.Location = CardLocation.Hand;
                            //player1.ResetCardPosition(selectedCard);
                            selectedCard.ResetCard();
                            selectedCard = null;
                            //Console.WriteLine("released");
                            startingLocation = null;
                        }
                        
                    }
                }
            }
            else
            {
                if (selectedCard != null && target != Location.Cards) //check where to place card when mouse is released and if no valid slot is selected then drop the card back to previous spot
                {
                    if (selectedCard.Location == CardLocation.Moving)
                    {
                        selectedCard.Selected = false;
                        selectedCard.Location = CardLocation.Hand;
                        //player1.ResetCardPosition(selectedCard);
                        selectedCard.ResetCard();
                        selectedCard = null;
                        //Console.WriteLine("released");
                        startingLocation = null;
                        //player1.ResetCards();
                    }

                }
                else
                {
                    if (selectedCard != null && target == Location.Cards)
                    {
                        activeDeck.savedCards.Remove(selectedCard);
                        Database.RemoveCardFromDeck(player, activeDeck, selectedCard);
                        //cards.Add(selectedCard);
                        SetAvailableCards();
                        SetDeckCardPositions();
                        selectedCard = null;
                        //activeDeck.AddCardToSavedDeck(selectedCard);
                        //selectedCard.Selected = false;
                        //selectedCard.Location = CardLocation.Hand;
                        ////player1.ResetCardPosition(selectedCard);
                        ////selectedCard.ResetCard();
                        //selectedCard = null;
                    }
                }
            }
            

        }

        private Location? GetTarget(Vector2i mouse)
        {

            if (deckBoundingBox.GetGlobalBounds().Contains(mouse.X, mouse.Y))
            {
                return Location.Deck;
            }
            else if (cardsBoundingBox.GetGlobalBounds().Contains(mouse.X, mouse.Y))
            {
                return Location.Cards;
            }
            //check two bounding boxes on in the cards view other in deck view
            return null;
        }


        public void Update(System.TimeSpan time)
        {
            foreach (var button in deckSelectButtons)
            {
                button.Update(time);
            }
            foreach (var button in deckEditButtons)
            {
                button.Update(time);
            }

        }

        public void Render()
        {
            DrawBackground();
            if (managerState == ManagerState.DeckSelection)
            {
                DrawDeckSelect();
            }
            else
            {
                DrawDeckEdit();
            }
        }

        private void InitiallizeShapes()
        {
            shapes = new List<Shape>();
            shapes.Add(new RectangleShape(new Vector2f(Game.ScreenWidth * 4, Game.ScreenHeight * 4)) { FillColor = Color.White, Position = new Vector2f(-1 * (Game.ScreenWidth * 2) / 2, -1 * (Game.ScreenHeight * 2) / 2) });
        }

        private void InitiallizeDeckEditText()
        {

            deckEditText.Add(HelperFunctions.NewText("Cards", 30, new Vector2f(Game.ScreenWidth * 0.8f, 50), Color.Black));
            deckEditText.Add(HelperFunctions.NewText("Deck", 30, new Vector2f(Game.ScreenWidth * 0.4f, 50), Color.Black));
            for (int i = 0; i < deckEditText.Count; i++)
            {
                deckEditText[i].Origin = new Vector2f(deckEditText[i].GetGlobalBounds().Width / 2, deckEditText[i].GetGlobalBounds().Height / 2);
            }


        }
        private void InitiallizeDeckSelectText()
        {

            deckSelectText.Add(HelperFunctions.NewText("Select a Deck", 30, new Vector2f((Game.ScreenWidth) / 2, 60), Color.Black));
            //deckEditText.Add(HelperFunctions.NewText("Decks", 30, new Vector2f((Game.ScreenWidth) / 2 * -1 + 50, Game.ScreenHeight / 2 * -1 + 100), Color.Black));
            for (int i = 0; i < deckSelectText.Count; i++)
            {
                deckSelectText[i].Origin = new Vector2f(deckEditText[i].GetGlobalBounds().Width / 2, deckEditText[i].GetGlobalBounds().Height / 2);
            }


        }

        private void InitiallizeButtons()
        {
            deckSelectButtons.Add(new Button("Create Deck", 40, new Vector2f(500, 50), Color.Black, CreateDeck));
            deckEditButtons.Add(new Button("Exit", 40, new Vector2f(200, 50), Color.Black, () => { SaveDeck(); managerState = ManagerState.DeckSelection; }));


        }

        private void SaveDeck()
        {

        }

        private void DrawDeckSelect()
        {
            //window.SetView(Game.defaultView);
            window.Draw(title);
            foreach (var text in deckSelectText)
            {
                window.Draw(text);
            }
            //draw decks to pick
            foreach (var button in deckSelectButtons)
            {
                window.Draw(button);
            }

            if (player.decks != null)
            {

            }
        }

        private void DrawDeckEdit()
        {
            window.SetView(Cardsview);

            foreach (var card in cards)
            {
                if (selectedCard != null)
                {
                    if (card.Equals(selectedCard))
                    {
                        continue;
                    }
                }

                card.viewType = Field.ViewType.FieldView;
                window.Draw(card);
            }

            window.SetView(deckView);

            foreach (var card in activeDeck.savedCards)
            {
                if (selectedCard != null)
                {
                    if (card.Equals(selectedCard))
                    {
                        continue;
                    }
                }

                card.viewType = Field.ViewType.FieldView;
                window.Draw(card);
            }

            window.SetView(CardView);
            if (lastSelectedCard != null)
            {
                lastSelectedCard.viewType = Field.ViewType.SideView;
                window.Draw(lastSelectedCard);

            }



            window.SetView(Game.defaultView);
            foreach (var button in deckEditButtons)
            {
                window.Draw(button);
            }
            foreach (var text in deckEditText)
            {
                window.Draw(text);
            }
            window.Draw(deckBoundingBox);
            window.Draw(cardsBoundingBox);

            window.SetView(DeckEditView);
            if (selectedCard != null)
            {
                selectedCard.viewType = Field.ViewType.FieldView;
                window.Draw(selectedCard);
            }

        }
        private void DrawBackground()
        {
            window.SetView(Game.defaultView);
            foreach (var shape in shapes)
            {
                window.Draw(shape);
            }

        }
    }
}
