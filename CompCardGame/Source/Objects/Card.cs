using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompCardGame.Source.Field;
using CompCardGame.Source.Core;

namespace CompCardGame.Source.Objects
{


    class Card : Transformable, Drawable, ICard
    {


        //shapes used to draw the card
        private Shape[] shapes;

        public RectangleShape boundingBox;
        //might need getter and a setter so we can modify card color that is displayed when this is changed
        //public Color color;
        //public Color accentColor;

        RectangleShape backSide;
        //card attributes
        public Text cardName;
        Text cardDescription;
        //Text cardAttackText;
        //Text cardDefenseText;
        //Text cardManaText;
        //Text cardMaxManaText;
        //Text cardCrystalCostText;

        public Vector2f previousPosition;
        //used to know which side to draw
        private CardState state = CardState.Back;

        public ViewType viewType = ViewType.FieldView;

        //private int attack;
        //private int defense;
        //private int mana;
        //private int maxMana
        private int crystalCost;
        //public int attackManaCost;

        private List<CircleShape> crystals;

        public const float width = 200f;
        public const float height = 320f;

        public readonly int id;

        public readonly int ingameID;
        //Selected is only true when mouse is hovered over the card in hand or if selected while on the field
        public Boolean Selected { get; set; }
        //card location can be either deck, hand, field, or graveyard
        public CardLocation Location { get; set; }

        
        public CardState State { get { return state; } set { state = value; } }
        //public int Attack { get { return attack; } set { attack = value; cardAttackText.DisplayedString = "Attack: " + value.ToString(); } }
        //public int Defense { get { return defense; } set { defense = value; cardDefenseText.DisplayedString = "Defense: " + value.ToString(); } }

        //public int Mana { get { return mana; } set { mana = value; cardManaText.DisplayedString = "ManaPool: " + value.ToString(); } }

        //public int MaxMana { get { return maxMana; } set { maxMana = value; cardMaxManaText.DisplayedString = "MaxMana: " + value.ToString(); } }//future it will be shown by top filled mana pool icon

        public int CrystalCost
        {
            get { return crystalCost; }
            set { crystalCost = value; SetCrystals(value); }
        }
        //generic constructor will be rarely used was mainly for testing
        public Card()
        {
            
            ingameID = HelperFunctions.random.Next();
            //Console.WriteLine(ingameID);
            //creating the shapes of the card
            var color = Color.Cyan;
            var accentColor = Color.Black;
            shapes = CardShapes(Position, color, accentColor);
            backSide = new RectangleShape(new Vector2f(width, height)) { FillColor = new Color(139, 69, 10), OutlineColor = new Color(169, 169, 169), OutlineThickness = 2 };
            boundingBox = new RectangleShape(new Vector2f(width, height)) { FillColor = Color.Transparent, OutlineThickness = 2, OutlineColor = Color.Red };
            //filling in the name and description
            cardName = HelperFunctions.NewText("Card Name", 15, new Vector2f { X = 10f, Y = 10f }, Color.Black);
            cardDescription = HelperFunctions.NewText("Description", 10, new Vector2f { X = 10f, Y = 235f }, Color.Black);
            //cardAttackText = HelperFunctions.NewText("Attack: ", 15, new Vector2f { X = 5f, Y = height - 20f }, Color.Black);
            //cardDefenseText = HelperFunctions.NewText("Defense: ", 15,  new Vector2f { X = 100f, Y = height - 20f }, Color.Black);
            //cardManaText = HelperFunctions.NewText("ManaPool: ", 15,  new Vector2f { X = 5f, Y = height - 35f }, Color.Black);
            //cardMaxManaText = HelperFunctions.NewText("MaxMana: ", 15,  new Vector2f { X = 100f, Y = height - 35f }, Color.Black);
            //cardCrystalCostText = HelperFunctions.NewText("CrystalCost: ", 15, new Vector2f { X = 10f, Y = 30f }, Color.Black);
            //attributes for dealing damage and defense yugioh does in hundreds, Hearthstone is in singles digits not sure which to use
            //Attack  = 100;
            //Defense = 100;
            //Mana = 1;
            //MaxMana = 1;
            crystals = new List<CircleShape>();
            CrystalCost = 2;
            
            //attackManaCost = 1;

            //for (int i = 0; i < CrystalCost; i++)
            //{
            //    crystals.Add(new CircleShape(5f, 4) { Position = new Vector2f(10f + i * 15f, 35f), FillColor = Color.Magenta, OutlineColor = new Color(169, 169, 169), OutlineThickness = 1 });
            //}

            Selected = false;
            Location = CardLocation.Deck;

        }

        public Card(Card card)
        {
            id = card.id;
            ingameID = card.ingameID;

            
            //creating the shapes of the card
            var color = Color.Cyan;
            var accentColor = Color.Black;
            shapes = CardShapes(Position, color, accentColor);
            backSide = new RectangleShape(new Vector2f(width, height)) { FillColor = new Color(139, 69, 10), OutlineColor = new Color(169, 169, 169), OutlineThickness = 2 };
            boundingBox = new RectangleShape(new Vector2f(width, height)) { FillColor = Color.Transparent, OutlineThickness = 2, OutlineColor = Color.Red };
            //filling in the name and description
            cardName = HelperFunctions.NewText(card.cardName.DisplayedString, 15, new Vector2f { X = 10f, Y = 10f }, Color.Black);
            cardDescription = HelperFunctions.NewText(card.cardDescription.DisplayedString, 10, new Vector2f { X = 10f, Y = 235f }, Color.Black);
            //cardAttackText = HelperFunctions.NewText("Attack: ", 15, new Vector2f { X = 5f, Y = height - 20f }, Color.Black);
            //cardDefenseText = HelperFunctions.NewText("Defense: ", 15,  new Vector2f { X = 100f, Y = height - 20f }, Color.Black);
            //cardManaText = HelperFunctions.NewText("ManaPool: ", 15,  new Vector2f { X = 5f, Y = height - 35f }, Color.Black);
            //cardMaxManaText = HelperFunctions.NewText("MaxMana: ", 15,  new Vector2f { X = 100f, Y = height - 35f }, Color.Black);
            //cardCrystalCostText = HelperFunctions.NewText("CrystalCost: ", 15, new Vector2f { X = 10f, Y = 30f }, Color.Black);
            //attributes for dealing damage and defense yugioh does in hundreds, Hearthstone is in singles digits not sure which to use
            //Attack  = 100;
            //Defense = 100;
            //Mana = 1;
            //MaxMana = 1;
            crystals = card.crystals;
            CrystalCost = card.CrystalCost;
            State = card.State;
            //attackManaCost = 1;

            //for (int i = 0; i < CrystalCost; i++)
            //{
            //    crystals.Add(new CircleShape(5f, 4) { Position = new Vector2f(10f + i * 15f, 35f), FillColor = Color.Magenta, OutlineColor = new Color(169, 169, 169), OutlineThickness = 1 });
            //}

            Selected = false;
            Location = CardLocation.Deck;
        }

        
        public Card(int id)
        {
            this.id = id;
            ingameID = HelperFunctions.random.Next();
            //creating the shapes of the card
            var color = Color.Cyan;
            var accentColor = Color.Black;
            shapes = CardShapes(Position, color, accentColor);
            backSide = new RectangleShape(new Vector2f(width, height)) { FillColor = new Color(139, 69, 10), OutlineColor = new Color(169, 169, 169), OutlineThickness = 2 };
            boundingBox = new RectangleShape(new Vector2f(width, height)) { FillColor = Color.Transparent, OutlineThickness = 2, OutlineColor = Color.Red };
            //filling in the name and description
            cardName = HelperFunctions.NewText("Card Name", 15, new Vector2f { X = 10f, Y = 10f }, Color.Black);
            cardDescription = HelperFunctions.NewText("Description", 10, new Vector2f { X = 10f, Y = 235f }, Color.Black);
            //cardAttackText = HelperFunctions.NewText("Attack: ", 15, new Vector2f { X = 5f, Y = height - 20f }, Color.Black);
            //cardDefenseText = HelperFunctions.NewText("Defense: ", 15,  new Vector2f { X = 100f, Y = height - 20f }, Color.Black);
            //cardManaText = HelperFunctions.NewText("ManaPool: ", 15,  new Vector2f { X = 5f, Y = height - 35f }, Color.Black);
            //cardMaxManaText = HelperFunctions.NewText("MaxMana: ", 15,  new Vector2f { X = 100f, Y = height - 35f }, Color.Black);
            //cardCrystalCostText = HelperFunctions.NewText("CrystalCost: ", 15, new Vector2f { X = 10f, Y = 30f }, Color.Black);
            //attributes for dealing damage and defense yugioh does in hundreds, Hearthstone is in singles digits not sure which to use
            //Attack  = 100;
            //Defense = 100;
            //Mana = 1;
            //MaxMana = 1;
            crystals = new List<CircleShape>();
            CrystalCost = 2;

            //attackManaCost = 1;

            //for (int i = 0; i < CrystalCost; i++)
            //{
            //    crystals.Add(new CircleShape(5f, 4) { Position = new Vector2f(10f + i * 15f, 35f), FillColor = Color.Magenta, OutlineColor = new Color(169, 169, 169), OutlineThickness = 1 });
            //}

            Selected = false;
            Location = CardLocation.Deck;

        }

        public Card(String name, String discription)
        {
            
            ingameID = HelperFunctions.random.Next();
            //creating the shapes of the card
            shapes = CardShapes(Position, Color.Cyan, Color.Black);
            backSide = new RectangleShape(new Vector2f(width, height)) { FillColor = new Color(139, 69, 10), OutlineColor = new Color(169, 169, 169), OutlineThickness = 2 };
            boundingBox = new RectangleShape(new Vector2f(width, height)) { FillColor = Color.Transparent, OutlineThickness = 1, OutlineColor = Color.Red };
            //filling in the name and description
            cardName = HelperFunctions.NewText(name, 15, Position + new Vector2f { X = 10f, Y = 10f }, Color.Black);
            cardDescription = HelperFunctions.NewText(discription, 10, Position + new Vector2f { X = 10f, Y = 235f }, Color.Black);
            //cardAttackText = HelperFunctions.NewText("Attack: ", 15, Position + new Vector2f { X = 10f, Y = height - 20f }, Color.Black);
            //cardDefenseText = HelperFunctions.NewText("Defense: ", 15, Position + new Vector2f { X = 100f, Y = height - 20f }, Color.Black);


            //Attack = attack;
            //Defense = defense;
            Selected = false;
            Location = CardLocation.Deck;
        }


        public bool Equals(Card obj)
        {
            //Console.WriteLine($"{obj.ingameID} {this.ingameID}");

            return obj.ingameID == this.ingameID;
        }

        public virtual void Draw(RenderTarget target, RenderStates states)
        {
           
            if (viewType == ViewType.SideView)
            {
                
                    states.Transform.Translate(new Vector2f(Game.SideViewWidth - Card.width * 1.4f, Game.ScreenHeight - Card.height * 1.4f - 50));
                    //drawing each of the shapes
                    foreach (var shape in shapes)
                    {
                        target.Draw(shape, states);
                    }
                    //drawing the text
                    target.Draw(cardName, states);
                    target.Draw(cardDescription, states);
                    //target.Draw(cardAttackText, states);
                    //target.Draw(cardDefenseText, states);
                    //target.Draw(cardManaText, states);
                    //target.Draw(cardMaxManaText, states);

                    for (int i = 0; i < crystals.Count; i++)
                    {
                        target.Draw(crystals[i], states);
                    }
                    //target.Draw(cardCrystalCostText, states);
                
            }
            else
            {


                //applying object transform to the states transform for drawing uniformly
                states.Transform = Transform;
                //drawing based on the state
                if (state == CardState.Front)
                {
                    //drawing each of the shapes
                    foreach (var shape in shapes)
                    {
                        target.Draw(shape, states);
                    }
                    //drawing the text
                    target.Draw(cardName, states);
                    target.Draw(cardDescription, states);
                    //target.Draw(cardAttackText, states);
                    //target.Draw(cardDefenseText, states);
                    //target.Draw(cardManaText, states);
                    //target.Draw(cardMaxManaText, states);
                    //target.Draw(boundingBox);
                    for (int i = 0; i < crystals.Count; i++)
                    {
                        target.Draw(crystals[i], states);
                    }
                    //target.Draw(cardCrystalCostText, states);
                }
                else //drawing the back
                {
                    target.Draw(backSide, states);
                }
                if (Selected)
                {
                    target.Draw(boundingBox);
                }
            }
        }
        //this is for checking bounding box of card
        public Boolean Contains(Vector2f point)
        {

            return boundingBox.GetGlobalBounds().Contains(point.X, point.Y);

        }

        public void SetCrystals(int crystalCount)
        {
            crystals.Clear();
            for (int i = 0; i < crystalCount; i++)
            {
                crystals.Add(new CircleShape(5f, 4)
                { Position = new Vector2f(10f + i * 15f, 35f), FillColor = Color.Magenta, OutlineColor = new Color(169, 169, 169), OutlineThickness = 1 });
            }
        }

        public void SetColors(Color baseColor, Color accentColor, Color textColor)
        {

            shapes[0].FillColor = baseColor;
            foreach (var shape in shapes)
            {
                shape.OutlineColor = accentColor;
            }
        }

        public void SetColors(Color baseColor, Color accentColor)
        {

            shapes[0].FillColor = baseColor;
            foreach (var shape in shapes)
            {
                shape.OutlineColor = accentColor;
            }
        }

        //this is to update the boundingbox might do more in the future
        public void UpdatePositions(Vector2f mouse)//position is mouse + offset of card so grabbing center of card
        {
            if (Game.GameState == GameState.CardManager)
            {
                this.Position = new Vector2f(mouse.X, mouse.Y);
                boundingBox.Position = Position;
            }
            else
            {
                this.Position = new Vector2f(mouse.X - width / 2, mouse.Y - height / 2);
                boundingBox.Position = Position;
            }
           
        }
        public void UpdatePositions()
        {
            boundingBox.Position = Position;
        }
        //raise card up when being looked at
        public void LiftCardUp()
        {
            //this is only done when card is inactive and it is in the hand
            if (!Selected && Location == CardLocation.Hand)
            {
                Position -= new Vector2f(0, height / 1.5f + 20);
                UpdatePositions();
                Selected = true;
            }

        }

        public void ResetCard()
        {
            Position = previousPosition;
            UpdatePositions();
        }
        //lower card back down after looking at it
        public void SetCardDown()
        {
            if (Location == CardLocation.Hand)
            {
                Position += new Vector2f(0, height / 1.5f + 20);
                UpdatePositions();
                Selected = false;
            }

        }

        private static Shape[] CardShapes(Vector2f position, Color fillColor, Color accentColor)
        {
            Shape[] shapes = new Shape[3];
            //creating the shapes of the card
            shapes[0] = new RectangleShape(new Vector2f { X = width, Y = height }) { FillColor = fillColor, OutlineColor = accentColor, OutlineThickness = 2f };
            shapes[1] = new RectangleShape(new Vector2f { X = width - 20f, Y = 20f }) { OutlineColor = accentColor, OutlineThickness = 2f, Position = position + new Vector2f { X = 10f, Y = 10f } };
            shapes[2] = new RectangleShape(new Vector2f { X = width - 20f, Y = width - 20f }) { OutlineColor = accentColor, OutlineThickness = 2f, Position = position + new Vector2f { X = 10f, Y = 50f } };

            return shapes;
        }



    }
}
