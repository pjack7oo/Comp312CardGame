using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Crystal_Wars.Source.Core.Models
{
    [BsonIgnoreExtraElements]
    class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        
        public List<Deck> Decks { get; set; }
        public List<Card_Instance> Cards { get; set; }
    }

    [BsonIgnoreExtraElements]
    class Deck
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public bool Active { get; set; }
        public List<BsonDocument> Card_Instance_Ids { get; set; }
    }

    [BsonIgnoreExtraElements]
    class Card_Instance
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        
        public ObjectId Card_Id { get; set; }
    }

    
}
