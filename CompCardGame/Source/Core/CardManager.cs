using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompCardGame.Source.Objects;

namespace CompCardGame.Source.Core
{
    class CardManager
    {

        private enum ManagerState
        {
            DeckSelection,
            DeckEdit
        }
        private RenderWindow window;

        private Text title;
        private List<Text> deckEditText = new List<Text>();
        private List<Text> deckSelectText = new List<Text>();
        private Player player;
        private View Cardsview;
        private View CardView;
        private View deckView;
        private List<Shape> shapes;
        private Deck activeDeck;
        
        private List<Button> deckSelectButtons;
        private List<Button> deckEditButtons;

        private ManagerState managerState = ManagerState.DeckSelection;

        public CardManager(RenderWindow renderWindow, Player player)
        {
            window = renderWindow;
            title = HelperFunctions.NewText("Card and Deck Manager", 40, new Vector2f(Game.ScreenWidth / 2, 20), new Color(101, 67, 33));
            title.Origin = new Vector2f(title.GetGlobalBounds().Width/2, title.GetGlobalBounds().Height/2);
            this.player = player;

            Cardsview = new View(new FloatRect(0f, 0f, Game.ScreenWidth, Game.ScreenHeight));
            Cardsview.Zoom(2f);
            Cardsview.Viewport = new FloatRect(0.6f, 0f, 1f, 1f);

            deckView = new View(new FloatRect(0f, 0f, Game.ScreenWidth, Game.ScreenHeight));
            deckView.Zoom(2f);
            deckView.Viewport = new FloatRect(0.2f, 0f, 0.4f, 1f);

            CardView = new View(new FloatRect(0f, 0f, Game.ScreenWidth / 4.8f, Game.ScreenHeight));
            CardView.Zoom(0.8f);
            CardView.Viewport = new FloatRect(0f, 0f, 0.2f, 1f);

            deckSelectButtons = new List<Button>();
            deckEditButtons = new List<Button>();



            InitiallizeShapes();
            InitiallizeDeckEditText();
            InitiallizeDeckSelectText();
            InitiallizeButtons();
            player.cards = new List<Card>();
            //Temporary 
            
            int tempI = 0;
            //temporary for loop for testing later wont be needed because it will load in the players cards
            for (int i = 0; i < 60; i++)
            {
                if (i % 4 == 0)
                {
                    if (tempI == 0)
                    {
                        var card = new SpellCard();
                        card.cardName.DisplayedString = $"{i}";
                        card.State = CardState.Front;
                        player.cards.Add(card);
                        tempI = 1;
                    }
                    else
                    {
                        var card = new SpellCard();
                        card.SetEffect(Effect.OverloadCardMana(2, card));
                        card.cardName.DisplayedString = $"Overload Mana";
                        card.State = CardState.Front;
                        player.cards.Add(card);
                        tempI = 0;
                    }
                }
                else
                {
                    if (i %3 == 0)
                    {
                        var card = new EffectMonster() { MaxMana = 2 };
                        var effects = new Effect[2];
                        effects[0] = Effect.HealPlayer(5, card, 1, 2);
                        effects[1] = Effect.OverloadCardMana(1, card, 2, 0, true);
                        //card.SetEffect(Effect.HealPlayer(5, card), 2);
                        card.SetEffects(effects);
                        card.cardName.DisplayedString = $"{i}";
                        card.State = CardState.Front;
                        card.Attack = 110;
                        player.cards.Add(card);
                    }
                    else
                    {
                        var card = new MonsterCard();
                        card.cardName.DisplayedString = $"{i}";
                        card.State = CardState.Front;
                        card.Attack = 110;
                        player.cards.Add(card);
                    }
                }
            }
            //not temp
            OrganizeCards();
            SetCardPositions();
        }

        //algorithm to organize cards 
        private void OrganizeCards()
        {
            
        }

        public void AddButton(Button button)
        {
            deckSelectButtons.Add(button);
        }

        private void SetCardPositions()
        {
            var length = player.cards.Count;
            var cards = player.cards;
            var y = 0;
            var x = 0;
            for (int i = 0; i <length;i++)
            {
                if (i % 6 == 0 && i != 0)
                {
                    y++;
                    x = 0;
                }
                cards[i].Origin = new Vector2f(Card.width / 2, Card.height / 2);
                cards[i].Position = new Vector2f(x*(Card.width+40)-800, y*(Card.height+20) + Game.ScreenHeight / 2 * -1 + 125+deckEditText[1].GetGlobalBounds().Height+Card.height/2);

                x++;

                
            }
        }

        private void CreateDeck()
        {
            var deck = new Deck();
            player.decks.Add(deck);
            CreateDeckButton(deck);
        }

        private void CreateDeckButton(Deck deck)
        {
            
            var button = new Button("", 0, new Vector2f((player.decks.Count - 1) * (Card.width + 20) + 150, 400), Color.Black, new RectangleShape(new Vector2f(Card.width,Card.height)) { FillColor = new Color(139, 69, 10), OutlineColor = new Color(169, 169, 169), OutlineThickness = 2 }, ()=> { activeDeck = deck; managerState = ManagerState.DeckEdit; });
            deckSelectButtons.Add(button);
        }

        public void MouseClick(Vector2f mouse)
        {
            if (managerState == ManagerState.DeckSelection)
            {
                for (int i = 0; i < deckSelectButtons.Count; i++)
                {
                    if (deckSelectButtons[i].Contains(mouse))
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
                    if (deckEditButtons[i].Contains(mouse))
                    {
                        deckEditButtons[i].EnableActionUse();
                        deckEditButtons[i].DoAction();
                    }
                }
            }
        }

        public void Update(System.TimeSpan time)
        {
            foreach(var button in deckSelectButtons)
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
            shapes.Add(new RectangleShape(new Vector2f(Game.ScreenWidth*4 , Game.ScreenHeight*4)) { FillColor = Color.White, Position = new Vector2f(-1*(Game.ScreenWidth * 2)/2,-1*(Game.ScreenHeight*2)/2 )});
        }

        private void InitiallizeDeckEditText()
        {
            
            deckEditText.Add(HelperFunctions.NewText("Cards", 30, new Vector2f( (Game.ScreenWidth)/2+50,Game.ScreenHeight/2*-1+100), Color.Black));
            deckEditText.Add(HelperFunctions.NewText("Decks", 30, new Vector2f((Game.ScreenWidth) / 2*-1 + 50, Game.ScreenHeight / 2 * -1 + 100), Color.Black));
            for(int i =0; i < deckEditText.Count;i++)
            {
                deckEditText[i].Origin = new Vector2f(deckEditText[i].GetGlobalBounds().Width / 2, deckEditText[i].GetGlobalBounds().Height / 2);
            }
            

        }
        private void InitiallizeDeckSelectText()
        {
            
            deckEditText.Add(HelperFunctions.NewText("Select a Deck", 30, new Vector2f((Game.ScreenWidth) / 2, 60), Color.Black));
            //deckEditText.Add(HelperFunctions.NewText("Decks", 30, new Vector2f((Game.ScreenWidth) / 2 * -1 + 50, Game.ScreenHeight / 2 * -1 + 100), Color.Black));
            for (int i = 0; i < deckEditText.Count; i++)
            {
                deckEditText[i].Origin = new Vector2f(deckEditText[i].GetGlobalBounds().Width / 2, deckEditText[i].GetGlobalBounds().Height / 2);
            }


        }

        private void InitiallizeButtons()
        {
            deckSelectButtons.Add(new Button("Create Deck", 40, new Vector2f(500, 50), Color.Black, CreateDeck));
            deckEditButtons.Add(new Button("Exit", 40, new Vector2f(200, 50), Color.Black, () => { SaveDeck();  managerState = ManagerState.DeckSelection; }));


        }

        private void SaveDeck()
        {

        }

        private void DrawDeckSelect()
        {
            //window.SetView(Game.defaultView);
            window.Draw(title);
            foreach (var text in deckEditText)
            {
                window.Draw(text);
            }
            //draw decks to pick
            foreach(var button in deckSelectButtons)
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
            
            foreach (var card in player.cards)
            {
                window.Draw(card);
            }
            window.SetView(Game.defaultView);
            foreach ( var button in deckEditButtons)
            {
                window.Draw(button);
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
