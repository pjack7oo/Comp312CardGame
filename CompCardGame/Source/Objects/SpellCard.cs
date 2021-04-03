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
    class SpellCard: Card
    {
        public readonly bool isFieldType;
        public Effect[] effects;

        private readonly Text fieldTypeText;//temporary later this will be a symbol on the card
        public Boolean Active { get; private set; }

        //public bool DrawEffectButtons { get; set; }
        public SpellCard(): base()
        {
            SetColors(new Color(208,0,208), Color.Black);
            isFieldType = false;
            fieldTypeText = new Text(isFieldType ? "FieldCard" : "", HelperFunctions.font, 15) { FillColor = Color.Black, Position = new Vector2f(Card.width - 82,30)};
            effects = new Effect[1];
            effects[0] = new Effect(this);    
        }
        public SpellCard(int i) : base(i)
        {
            
            SetColors(new Color(208, 0, 208), Color.Black);
            isFieldType = false;
            fieldTypeText = new Text(isFieldType ? "FieldCard" : "", HelperFunctions.font, 15) { FillColor = Color.Black, Position = new Vector2f(Card.width - 82, 30) };
            effects = new Effect[1];
            effects[0] = Effect.HealAllyCard(5, this);//temporary for testing
        }
        public SpellCard(bool isFieldType) : base()
        {
            SetColors(new Color(208, 0, 208), Color.Black);
            this.isFieldType = isFieldType;
            effects = new Effect[1];
            
            fieldTypeText = new Text(isFieldType ? "FieldCard" : "", HelperFunctions.font, 50);
            
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

        public SpellCard(Effect[] effects, bool isFieldType) : base()
        {
            SetColors(new Color(208, 0, 208), Color.Black);
            this.isFieldType = isFieldType;
            this.effects = effects;
            fieldTypeText = new Text(isFieldType ? "FieldCard" : "", HelperFunctions.font, 50);
            
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            
            base.Draw(target, states);
           
            if (viewType == ViewType.SideView)
            {
                
                    states.Transform.Translate(new Vector2f(Game.SideViewWidth - Card.width * 1.4f, Game.ScreenHeight - Card.height * 1.4f - 50));
                //foreach(var effect in effects)
                //{
                //    //target.Draw(effect, states);
                //}
                
                target.Draw(fieldTypeText, states);
                
                foreach(var effect in effects)
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
                    
                    //foreach (var effect in effects)
                    //{
                    //    //target.Draw(effect, states);
                    //}
                    target.Draw(fieldTypeText, states);
                    foreach (var effect in effects)
                    {
                        effect.viewType = viewType;
                        target.Draw(effect, states);
                    }
                }

            }
        }

        //public void ActivateEffect(int effectNumber)
        //{
        //    //if (effects[effectNumber].CanBeUsed())
        //    //{

        //    //}
        //}
        public Boolean CheckButtonClick(Vector2f mouse)
        {
            foreach(var effect in effects)
            {
                if (effect.CheckClick(mouse))
                {
                    //Console.WriteLine("true");
                    effect.DoAction();
                    return true;
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
        

        
    }
}
