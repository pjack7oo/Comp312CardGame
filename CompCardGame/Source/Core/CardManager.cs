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
        private RenderWindow window;

        private Text title;
        private List<Text> pageText = new List<Text>(); 
        private Player player;
        private View Cardsview;
        private View deckView;
        private List<Shape> shapes;
        public CardManager(RenderWindow renderWindow, Player player)
        {
            window = renderWindow;
            title = HelperFunctions.NewText("Card and Deck Manager", 40, new Vector2f(Game.ScreenWidth / 2, -1*(Game.ScreenHeight)/2+20), new Color(101, 67, 33));
            title.Origin = new Vector2f(title.GetGlobalBounds().Width/2, title.GetGlobalBounds().Height/2);
            this.player = player;
            Cardsview = new View(new FloatRect(0f, 0f, Game.ScreenWidth, Game.ScreenHeight));
            Cardsview.Zoom(2f);
            InitiallizeShapes();
            InitiallizeText();
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

        private void SetCardPositions()
        {
            var length = player.cards.Count;
            var cards = player.cards;
            var y = 0;
            var x = 0;
            for (int i = 0; i <length;i++)
            {
                if (i % 8 == 0 && i != 0)
                {
                    y++;
                    x = 0;
                }
                cards[i].Origin = new Vector2f(Card.width / 2, Card.height / 2);
                cards[i].Position = new Vector2f(x*(Card.width+40)+(Game.ScreenWidth)/2+100, y*(Card.height+20) + Game.ScreenHeight / 2 * -1 + 125+pageText[1].GetGlobalBounds().Height+Card.height/2);

                x++;

                
            }
        }

        public void Render()
        {
            DrawBackground();
        }

        private void InitiallizeShapes()
        {
            shapes = new List<Shape>();
            shapes.Add(new RectangleShape(new Vector2f(Game.ScreenWidth*4 , Game.ScreenHeight*4)) { FillColor = Color.White, Position = new Vector2f(-1*(Game.ScreenWidth * 2)/2,-1*(Game.ScreenHeight*2)/2 )});
        }

        private void InitiallizeText()
        {
            pageText.Add(HelperFunctions.NewText("Cards", 30, new Vector2f( (Game.ScreenWidth)/2+50,Game.ScreenHeight/2*-1+100), Color.Black));
            pageText.Add(HelperFunctions.NewText("Decks", 30, new Vector2f((Game.ScreenWidth) / 2*-1 + 50, Game.ScreenHeight / 2 * -1 + 100), Color.Black));
            for(int i =0; i < pageText.Count;i++)
            {
                pageText[i].Origin = new Vector2f(pageText[i].GetGlobalBounds().Width / 2, pageText[i].GetGlobalBounds().Height / 2);
            }
            

        }



        private void DrawBackground()
        {
            window.SetView(Cardsview);
            foreach (var shape in shapes)
            {
                window.Draw(shape);
            }

            foreach(var card in player.cards)
            {
                window.Draw(card);
            }

            window.Draw(title);
            foreach (var text in pageText)
            {
                window.Draw(text);
            }

            window.SetView(Game.defaultView);
            
            
        }
    }
}
