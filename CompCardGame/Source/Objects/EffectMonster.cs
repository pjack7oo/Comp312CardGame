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
    class EffectMonster : MonsterCard
    {
        public Effect[] effects;


        public EffectMonster() : base()
        {
            SetColors(new Color(192, 192, 192), Color.Black);
            effects = new Effect[1];
            effects[0] = new Effect(this);

        }

        public EffectMonster(int id) : base(id)
        {
            SetColors(new Color(192, 192, 192), Color.Black);
            effects = new Effect[1];
            effects[0] = new Effect(this);

        }

        public EffectMonster(EffectMonster card): base(card)
        {
            SetColors(new Color(192, 192, 192), Color.Black);
            effects = card.effects;
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
