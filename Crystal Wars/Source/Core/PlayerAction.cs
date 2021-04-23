using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Crystal_Wars.Source.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    class PlayerAction
    {
        public enum ActionType
        {
            Move,
            Effect,
            Attack,
            NextPhase
        }
        public enum CardType
        {
            Monster,
            Spell
        }

        public ActionType Type { get; set; }

        public int? Item { get; set; }

        public CardType? ItemType { get; set; }

        public int? Target { get; set; }

        public int? Effect { get; set; }

        public PlayerAction(ActionType type,CardType? cardType = null, int? item = null, int? target = null, int? effect = null)
        {
            Type = type;
            this.ItemType = cardType;
            this.Item     = item;
            this.Target   = target;
            this.Effect   = effect;
        }

        public void SendAction()
        {
            Thread thread = new Thread(new ThreadStart(() => {
                Networking.SendData(JMessage.Serialize(JMessage.FromValue(this)));
            }));
            thread.Start();
        }

    }
}
