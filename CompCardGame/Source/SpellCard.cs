using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{
    class SpellCard: Card
    {
        private readonly bool isFieldType;
        public readonly List<Effect> effects = new List<Effect>();

        private readonly Text fieldTypeText;//temporary later this will be a symbol on the card
        public Boolean Active { get; private set; }

        public bool DrawEffectButtons { get; set; }
        public SpellCard(): base()
        {
            SetColors(new Color(208,0,208), Color.Black);
            isFieldType = true;
            fieldTypeText = new Text(isFieldType ? "FieldCard" : "", HelperFunctions.font, 15) { FillColor = Color.Black, Position = new Vector2f(Card.width - 82,30)};
            effects.Add(new Effect(this));    
        }
        public SpellCard(int i) : base(i)
        {
            
            SetColors(new Color(208, 0, 208), Color.Black);
            isFieldType = true;
            fieldTypeText = new Text(isFieldType ? "FieldCard" : "", HelperFunctions.font, 15) { FillColor = Color.Black, Position = new Vector2f(Card.width - 82, 30) };
            effects.Add(Effect.HealAllyCard(5, this));
        }
        public SpellCard(Effect effect,bool isFieldType) : base()
        {
            SetColors(new Color(208, 0, 208), Color.Black);
            this.isFieldType = isFieldType;
            effects.Add(effect);
            fieldTypeText = new Text(isFieldType ? "FieldCard" : "", HelperFunctions.font, 50);
            
        }

        public SpellCard(Effect[] effects, bool isFieldType) : base()
        {
            SetColors(new Color(208, 0, 208), Color.Black);
            this.isFieldType = isFieldType;
            this.effects.AddRange(effects);
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
                    
                    return true;
                }
            }
            return false;
        }
        public void AddEffectButtons()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].AddButton();
            }
        }

        public void ActivateEffectButtons()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].ActivateButton();
            }
        }

        public void DeactivateEffectButtons()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].DeactivateButtons();
            }
        }
        

        
    }
}
