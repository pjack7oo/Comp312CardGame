using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal_Wars.Source.Core.Models
{
    [BsonKnownTypes(typeof(Spell), typeof(Monster))]
    public class Card
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public int Crystal_Cost { get; set; }
        public string Card_Type { get; set; }

    }

    public class Spell : Card
    {

        public bool IsContinuous { get; set; }

        public List<string> Effects { get; set; }

    }
    [BsonKnownTypes(typeof(EffectMonster))]
    public class Monster : Card
    {
        public int Health { get; set; }
        public int Mana { get; set; }
        public int Max_Mana { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Attack_Mana_Cost { get; set; }

    }

    public class EffectMonster : Monster
    {
        public List<Effect> Effects { get; set; }
    }

    public class Effect
    {
        public string Name { get; set; }
        public int ManaCost { get; set; }

    }


}
