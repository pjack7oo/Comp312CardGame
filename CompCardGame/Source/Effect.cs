using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source
{
    class Effect
    {
        public FieldType TargetCard { get; private set; }

        public PlayerType TargetPlayer { get; private set; }

        public delegate void CustomAction(Card card = null, Player player = null);

        public CustomAction action;

        private Button button;

        public Effect()
        {
            action = (card, player) => { };
        }

        public void ActivateEffect(Player player)
        {
            action(player : player);
        }

        public void ActivateEffect(Card card)
        {
            action(card: card);
        }

        public void ActivateEffect(Player player, Card card)
        {
            action(card, player);
        }

        public void AddButton(int i)//int used to place in right spot in order
        {
            //Game.InputHandler.AddButton(new Button(SetSelectedEffect));
        }

        public void SetSelectedEffect()
        {
            Match.selectedEffect = this;
        }

        public void RemoveButton()
        {
            //Game.InputHandler.removeButton(button); //TODO
        }

    }
}
