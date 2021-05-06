using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystal_Wars.Source.Field;
using Crystal_Wars.Source.Core;
using Newtonsoft.Json;
namespace Crystal_Wars.Source.Objects
{

    [JsonObject(MemberSerialization.OptOut)]
    class MonsterCard : Card
    {


        private int attack;
        private int defense;
        private int mana;

        private int maxMana;
        private int hp;
        public int attackManaCost;

        [NonSerialized]
        private readonly Text cardAttackText;
        [NonSerialized]
        private readonly Text cardDefenseText;
        [NonSerialized]
        private readonly Text cardManaText;
        //[NonSerialized]
        //private readonly Text cardMaxManaText;
        [NonSerialized]
        private readonly Text cardHpText;
        [NonSerialized]
        public static Sprite[] icons = new Sprite[4];
        public int Attack { get { return attack; } set { attack = value; cardAttackText.DisplayedString =  value.ToString(); } }
        public int Defense { get { return defense; } set { defense = value; cardDefenseText.DisplayedString =  value.ToString(); } }
        public int Hp { get { return hp; } set { hp = value; cardHpText.DisplayedString = value.ToString(); } }
        public int Mana { get { return mana; } set { mana = value; cardManaText.DisplayedString = value.ToString()+"/"+MaxMana.ToString(); } }

        public int ManaGain { get; set; }
        public int MaxMana { get { return maxMana; } set { maxMana = value; } }//future it will be shown by top filled mana pool icon
        public MonsterCard() : base()
        {
            //this.SetColors(new Color(210, 180, 140), Color.Black);
            CardIcons();
            face = new Sprite((Texture)FileHandler.GetItem("Gold_A_Texture"));
            nameBar = new Sprite((Texture)FileHandler.GetItem("Gold_B_Texture"), new IntRect(0, 0, (int)width - 20, 20)) { Position = new Vector2f { X = 10f, Y = 10f } };
            pictureBackground = new Sprite((Texture)FileHandler.GetItem("Gold_B_Texture"), new IntRect(0, 0, (int)width - 20, (int)width - 20)) { Position = new Vector2f { X = 10f, Y = 50f } };
            ScaleSprite(face);
            cardAttackText = HelperFunctions.NewText("100", 15, new Vector2f { X = 12f, Y = height - 20f }, Color.Black);
            cardDefenseText = HelperFunctions.NewText("100", 15, new Vector2f { X = width - 15f, Y = height - 20f }, Color.Black);
            cardManaText = HelperFunctions.NewText("1/1", 15, new Vector2f { X = 70f, Y = height - 10f }, Color.Black);
            //cardMaxManaText = HelperFunctions.NewText("1", 15, new Vector2f { X = 100f, Y = height - 35f }, Color.Black);
            cardHpText = HelperFunctions.NewText("10/10", 15, new Vector2f { X = 80f, Y = height - 28f }, Color.Black);

            cardAttackText.Origin = new Vector2f(cardAttackText.GetLocalBounds().Width / 2, cardAttackText.GetLocalBounds().Height / 2);
            cardDefenseText.Origin = new Vector2f(cardDefenseText.GetLocalBounds().Width / 2, cardDefenseText.GetLocalBounds().Height / 2);
            cardManaText.Origin = new Vector2f(cardManaText.GetLocalBounds().Width / 2, cardManaText.GetLocalBounds().Height / 2);
            //cardMaxManaText.Origin = new Vector2f(cardMaxManaText.GetLocalBounds().Width / 2, cardMaxManaText.GetLocalBounds().Height / 2);
            cardHpText.Origin = new Vector2f(cardHpText.GetLocalBounds().Width / 2, cardHpText.GetLocalBounds().Height / 2);

            Hp = 10;
            Attack = 10;
            Defense = 10;
            MaxMana = 1;
            Mana = 1;
            
            attackManaCost = 1;
            ManaGain = 1;
        }
        [JsonConstructor]
        public MonsterCard(int Hp, int Attack, int Defense, int Mana, int MaxMana, int attackManaCost, int ManaGain, int CrystalCost, string id, int ingameID, string CardName, string CardDescription, string pictureName) : base(CrystalCost, id, ingameID, CardName, CardDescription, pictureName)
        {
            //this.SetColors(new Color(210, 180, 140), Color.Black);
            CardIcons();
            face = new Sprite((Texture)FileHandler.GetItem("Gold_A_Texture"));
            nameBar = new Sprite((Texture)FileHandler.GetItem("Gold_B_Texture"), new IntRect(0, 0, (int)width - 20, 20)) { Position = new Vector2f { X = 10f, Y = 10f } };
            pictureBackground = new Sprite((Texture)FileHandler.GetItem("Gold_B_Texture"), new IntRect(0, 0, (int)width - 20, (int)width - 20)) { Position = new Vector2f { X = 10f, Y = 50f } };
            ScaleSprite(face);
            cardAttackText = HelperFunctions.NewText("100", 15, new Vector2f { X = 12f, Y = height - 20f }, Color.Black);
            cardDefenseText = HelperFunctions.NewText("100", 15, new Vector2f { X = width - 15f, Y = height - 20f }, Color.Black);
            cardManaText = HelperFunctions.NewText("1/1", 15, new Vector2f { X = 70f, Y = height - 10f }, Color.Black);
            //cardMaxManaText = HelperFunctions.NewText("1", 15, new Vector2f { X = 100f, Y = height - 35f }, Color.Black);
            cardHpText = HelperFunctions.NewText("10/10", 15, new Vector2f { X = 80f, Y = height - 28f }, Color.Black);


            cardAttackText.Origin = new Vector2f(cardAttackText.GetLocalBounds().Width / 2, cardAttackText.GetLocalBounds().Height / 2);
            cardDefenseText.Origin = new Vector2f(cardDefenseText.GetLocalBounds().Width / 2, cardDefenseText.GetLocalBounds().Height / 2);
            cardManaText.Origin = new Vector2f(cardManaText.GetLocalBounds().Width / 2, cardManaText.GetLocalBounds().Height / 2);
            //cardMaxManaText.Origin = new Vector2f(cardMaxManaText.GetLocalBounds().Width / 2, cardMaxManaText.GetLocalBounds().Height / 2);
            cardHpText.Origin = new Vector2f(cardHpText.GetLocalBounds().Width / 2, cardHpText.GetLocalBounds().Height / 2);

            this.Hp = Hp;
            this.Attack = Attack;
            this.Defense = Defense;
            this.MaxMana = MaxMana;
            this.Mana = Mana;
            
            this.attackManaCost = attackManaCost;
            this.ManaGain = ManaGain;
        }

        public MonsterCard(MonsterCard card) : base(card)
        {
            //this.SetColors(new Color(210, 180, 140), Color.Black);
            CardIcons();
            face = new Sprite((Texture)FileHandler.GetItem("Gold_A_Texture"));
            nameBar = new Sprite((Texture)FileHandler.GetItem("Gold_B_Texture"), new IntRect(0, 0, (int)width - 20, 20)) { Position = new Vector2f { X = 10f, Y = 10f } };
            pictureBackground = new Sprite((Texture)FileHandler.GetItem("Gold_B_Texture"), new IntRect(0, 0, (int)width - 20, (int)width - 20)) { Position = new Vector2f { X = 10f, Y = 50f } };
            ScaleSprite(face);
            cardAttackText = HelperFunctions.NewText("100", 15, new Vector2f { X = 12f, Y = height - 20f }, Color.Black);
            cardDefenseText = HelperFunctions.NewText("100", 15, new Vector2f { X = width - 15f, Y = height - 20f }, Color.Black);
            cardManaText = HelperFunctions.NewText("1/1", 15, new Vector2f { X = 70f, Y = height - 10f }, Color.Black);
            //cardMaxManaText = HelperFunctions.NewText("1", 15, new Vector2f { X = 100f, Y = height - 35f }, Color.Black);
            cardHpText = HelperFunctions.NewText("10/10", 15, new Vector2f { X = 80f, Y = height - 28f }, Color.Black);

            cardAttackText.Origin = new Vector2f(cardAttackText.GetLocalBounds().Width / 2, cardAttackText.GetLocalBounds().Height / 2);
            cardDefenseText.Origin = new Vector2f(cardDefenseText.GetLocalBounds().Width / 2, cardDefenseText.GetLocalBounds().Height / 2);
            cardManaText.Origin = new Vector2f(cardManaText.GetLocalBounds().Width / 2, cardManaText.GetLocalBounds().Height / 2);
            //cardMaxManaText.Origin = new Vector2f(cardMaxManaText.GetLocalBounds().Width / 2, cardMaxManaText.GetLocalBounds().Height / 2);
            cardHpText.Origin = new Vector2f(cardHpText.GetLocalBounds().Width / 2, cardHpText.GetLocalBounds().Height / 2);

            Hp = card.Hp;
            Attack = card.Attack;
            Defense = card.Defense;
            MaxMana = card.MaxMana;

            Mana = card.Mana;
            attackManaCost = card.attackManaCost;
            ManaGain = card.ManaGain;
        }
        public MonsterCard(string id) : base(id)
        {
            //this.SetColors(new Color(210, 180, 140), Color.Black);
            CardIcons();
            face = new Sprite((Texture)FileHandler.GetItem("Gold_A_Texture"));
            nameBar = new Sprite((Texture)FileHandler.GetItem("Gold_B_Texture"), new IntRect(0, 0, (int)width - 20, (int)width - 20)) { Position = new Vector2f { X = 10f, Y = 10f } };
            pictureBackground = new Sprite((Texture)FileHandler.GetItem("Gold_B_Texture"), new IntRect(0, 0, (int)width - 20, (int)width - 20)) { Position = new Vector2f { X = 10f, Y = 50f } };
            ScaleSprite(face);
            cardAttackText = HelperFunctions.NewText("100", 15, new Vector2f { X = 12f, Y = height - 20f }, Color.Black);
            cardDefenseText = HelperFunctions.NewText("100", 15, new Vector2f { X = width-15f, Y = height - 20f }, Color.Black);
            cardManaText = HelperFunctions.NewText("1/1", 15, new Vector2f { X = 70f, Y = height - 10f }, Color.Black);
            //cardMaxManaText = HelperFunctions.NewText("1", 15, new Vector2f { X = 100f, Y = height - 35f }, Color.Black);
            cardHpText = HelperFunctions.NewText("10/10", 15, new Vector2f { X = 80f, Y = height - 28f }, Color.Black);

            cardAttackText.Origin = new Vector2f(cardAttackText.GetLocalBounds().Width / 2, cardAttackText.GetLocalBounds().Height / 2);
            cardDefenseText.Origin = new Vector2f(cardDefenseText.GetLocalBounds().Width / 2, cardDefenseText.GetLocalBounds().Height / 2);
            cardManaText.Origin = new Vector2f(cardManaText.GetLocalBounds().Width / 2, cardManaText.GetLocalBounds().Height / 2);
            //cardMaxManaText.Origin = new Vector2f(cardMaxManaText.GetLocalBounds().Width / 2, cardMaxManaText.GetLocalBounds().Height / 2);
            cardHpText.Origin = new Vector2f(cardHpText.GetLocalBounds().Width / 2, cardHpText.GetLocalBounds().Height / 2);

            Hp = 10;
            Attack = 10;
            Defense = 10;
            MaxMana = 1;
            Mana = 1;
            
            attackManaCost = 1;
            ManaGain = 1;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
            if (viewType == ViewType.SideView)
            {
                if (State == CardState.Front)
                {
                    states.Transform.Translate(new Vector2f(Game.SideViewWidth - Card.width * 1.4f, Game.ScreenHeight - Card.height * 1.4f - 50));
                    foreach (var sprite in icons)
                    {
                        target.Draw(sprite, states);
                    }
                    //drawing each of the shapes
                    //foreach (var shape in shapes)
                    //{
                    //    target.Draw(shape, states);
                    //}
                    //drawing the text
                    //target.Draw(cardName, states);
                    //target.Draw(cardDescription, states);
                    target.Draw(cardAttackText, states);
                    target.Draw(cardDefenseText, states);
                    target.Draw(cardManaText, states);
                    //target.Draw(cardMaxManaText, states);
                    target.Draw(cardHpText, states);


                }
            }
            else
            {


                //applying object transform to the states transform for drawing uniformly
                states.Transform = Transform;
                //drawing based on the state
                if (State == CardState.Front)
                {
                    foreach (var sprite in icons)
                    {
                        target.Draw(sprite, states);
                    }

                    target.Draw(cardAttackText, states);
                    target.Draw(cardDefenseText, states);
                    target.Draw(cardManaText, states);
                    //target.Draw(cardMaxManaText, states);
                    target.Draw(cardHpText, states);



                }

            }
        }

        public virtual void GiveMana()
        {
            if (Mana < MaxMana)
            {
                Mana += ManaGain;
            }
            else if (Mana + ManaGain >= MaxMana)
            {
                Mana = MaxMana;
            }
        }


        private static void CardIcons()
        {
            icons[0] = ((Sprite)FileHandler.GetItem("Attack Icon"));
            icons[1] = ((Sprite)FileHandler.GetItem("Defense Icon"));
            icons[2] = ((Sprite)FileHandler.GetItem("HP Icon"));
            icons[3] = ((Sprite)FileHandler.GetItem("Mana Icon"));



            

            

            for (int i = 0;i <icons.Length;i++)
            {
                icons[i].Origin = new Vector2f(icons[i].GetLocalBounds().Width / 2, icons[i].GetLocalBounds().Height / 2);
                if (i ==0 || i ==1)
                {
                    ScaleIcon(icons[i]);
                }
                else
                {
                    ScaleIcon(icons[i],new Vector2f(0.025f,0.025f));
                }
            }
            icons[0].Position = new Vector2f(15, height-15);
            icons[1].Position = new Vector2f(width- 15, height-15);
            icons[2].Position = new Vector2f(50, height-24);
            icons[3].Position = new Vector2f(50,height-9);
        }

        private static void ScaleIcon(Sprite icon, Vector2f? scale = null)
        {
            if (scale == null)
            {
                scale = new Vector2f(0.05f, 0.05f);
            }
            icon.Scale = (Vector2f)scale;
        }


    }
}
