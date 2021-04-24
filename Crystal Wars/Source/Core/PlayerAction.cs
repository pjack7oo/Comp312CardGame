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
        [JsonConstructor]
        public PlayerAction(ActionType Type, CardType? ItemType = null, int? Item = null, int? Target = null, int? Effect = null)
        {
            this.Type = Type;
            this.ItemType = ItemType;
            this.Item     = Item;
            this.Target   = Target;
            this.Effect   = Effect;
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
