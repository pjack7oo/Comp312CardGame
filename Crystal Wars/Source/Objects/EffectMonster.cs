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
    class EffectMonster : MonsterCard
    {
        public Effect[] effects;


        public EffectMonster() : base()
        {
            //SetColors(new Color(192, 192, 192), Color.Black);
            face = new Sprite((Texture)FileHandler.GetItem("Silver_A_Texture"));
            nameBar = new Sprite((Texture)FileHandler.GetItem("Silver_B_Texture"), new IntRect(0, 0, (int)width - 20, 20)) { Position = new Vector2f { X = 10f, Y = 10f } };
            pictureBackground = new Sprite((Texture)FileHandler.GetItem("Silver_B_Texture"), new IntRect(0, 0, (int)width - 20, (int)width - 20)) { Position = new Vector2f { X = 10f, Y = 50f } };
            ScaleSprite(face);
            effects = new Effect[1];
            effects[0] = new Effect(1,this);

        }

        public EffectMonster(string id) : base(id)
        {
            //SetColors(new Color(192, 192, 192), Color.Black);
            face = new Sprite((Texture)FileHandler.GetItem("Silver_A_Texture"));
            nameBar = new Sprite((Texture)FileHandler.GetItem("Silver_B_Texture"), new IntRect(0, 0, (int)width - 20, 20)) { Position = new Vector2f { X = 10f, Y = 10f } };
            pictureBackground = new Sprite((Texture)FileHandler.GetItem("Silver_B_Texture"), new IntRect(0, 0, (int)width - 20, (int)width - 20)) { Position = new Vector2f { X = 10f, Y = 50f } };

            ScaleSprite(face);
            effects = new Effect[1];
            effects[0] = new Effect(1,this);

        }

        public EffectMonster(EffectMonster card): base(card)
        {
            //SetColors(new Color(192, 192, 192), Color.Black);
            face = new Sprite((Texture)FileHandler.GetItem("Silver_A_Texture"));
            nameBar = new Sprite((Texture)FileHandler.GetItem("Silver_B_Texture"), new IntRect(0, 0, (int)width - 20, 20)) { Position = new Vector2f { X = 10f, Y = 10f } };
            pictureBackground = new Sprite((Texture)FileHandler.GetItem("Silver_B_Texture"), new IntRect(0, 0, (int)width - 20, (int)width - 20)) { Position = new Vector2f { X = 10f, Y = 50f } };

            ScaleSprite(face);
            effects = card.effects;
        }
        [JsonConstructor]
        public EffectMonster(int Hp,int Attack, int Defense, int Mana, int MaxMana, int attackManaCost, int ManaGain,int CrystalCost, string id, int ingameID, string CardName, string CardDescription, string pictureName) : base(Hp,Attack,Defense,Mana,MaxMana,attackManaCost,ManaGain, CrystalCost, id,ingameID, CardName, CardDescription, pictureName)
        {
            //SetColors(new Color(192, 192, 192), Color.Black);
            face = new Sprite((Texture)FileHandler.GetItem("Silver_A_Texture"));
            nameBar = new Sprite((Texture)FileHandler.GetItem("Silver_B_Texture"), new IntRect(0, 0, (int)width - 20, 20)) { Position = new Vector2f { X = 10f, Y = 10f } };
            pictureBackground = new Sprite((Texture)FileHandler.GetItem("Silver_B_Texture"), new IntRect(0, 0, (int)width - 20, (int)width - 20)) { Position = new Vector2f { X = 10f, Y = 50f } };

            ScaleSprite(face);
        }

        public void SetEffect(Effect effect)
        {
            effects = new Effect[1];

            effects[0] = effect;


        }
        public void SetEffects(Effect[] effects)
        {
            this.effects = effects;



        }



        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
            if (viewType == ViewType.SideView)
            {

                states.Transform.Translate(new Vector2f(Game.SideViewWidth - Card.width * 1.4f, Game.ScreenHeight - Card.height * 1.4f - 50));


                foreach (var effect in effects)
                {
                    effect.viewType = viewType;
                    target.Draw(effect, states);
                }


            }
            else
            {


                //applying object transform to the states transform for drawing uniformly
                states.Transform = Transform;
                //drawing based on the state
                if (State == CardState.Front)
                {

                    foreach (var effect in effects)
                    {
                        effect.viewType = viewType;
                        target.Draw(effect, states);
                    }
                }

            }
        }

        public override void GiveMana()
        {
            foreach (var effect in effects)
            {
                effect.ResetUseAmount();
            }
            if (Mana < MaxMana)
            {
                Mana += ManaGain;
            }
            else if (Mana + ManaGain >= MaxMana)
            {
                Mana = MaxMana;
            }
        }

        public bool CheckButtonClick(Vector2f mouse)
        {
            foreach (var effect in effects)
            {
                if (effect.CheckClick(mouse))
                {

                    if (effect.CanUseEffect())
                    {

                        effect.DoAction();

                        return true;
                    }

                }
            }

            return false;
        }
        public void AddEffectButtons()
        {
            var len = effects.Length;
            for (int i = 0; i < len; i++)
            {
                effects[i].AddButton();
            }
        }

        public void ActivateEffectButtons()
        {
            var len = effects.Length;
            for (int i = 0; i < len; i++)
            {
                effects[i].ActivateButton();
            }
        }

        public void DeactivateEffectButtons()
        {
            var len = effects.Length;
            for (int i = 0; i < len; i++)
            {
                effects[i].DeactivateButtons();
            }
        }

        //public void UseMana(Effect effect)
        //{
        //    var len = effects.Length;
        //    for (int i = 0; i < len; i++)
        //    {
        //        if (effects[i].Equals(effect))
        //        {
        //            Console.WriteLine(effects[i].Equals(effect));
        //            Mana -= effectsCost[i];
        //            return;
        //        }
        //    }
        //}

        //public Boolean CanUseEffect(Effect effect)
        //{
        //    var len = effects.Length;
        //    for (int i = 0; i < len;i++)
        //    {
        //        if (effects[i].Equals(effect))
        //        {
        //            if (Mana - effects[i].effectCost >=0)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
    }
}
