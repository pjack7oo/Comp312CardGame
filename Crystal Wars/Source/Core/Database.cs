using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MongoDB.Driver;
using MongoDB.Bson;
using Crystal_Wars.Source.Objects;

namespace Crystal_Wars.Source.Core
{
    static class Database
    {
        private static MongoClient client = new MongoClient("mongodb+srv://root:comp312@cluster0.kjv42.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
        private static readonly string path = System.IO.Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.Personal), "CrystalWars");

        
        static public void DBConnectionTest()
        {
            var client = new MongoClient("mongodb+srv://root:comp312@cluster0.kjv42.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
            var dbList = client.ListDatabases().ToList();

            Console.WriteLine("The list of databases are:");
            foreach (var item in dbList)
            {
                Console.WriteLine(item);
            }
        }

        static public void RetrieveDocuments()
        {
            //csv file -- add more data later
            var collection = client.GetDatabase("cards").GetCollection<BsonDocument>("monsters");
            var documents = collection.Find(new BsonDocument()).ToList(); //achieve all documents

            //change document to string //print all for testing
            foreach (BsonDocument doc in documents) {
                Console.WriteLine(doc.ToString());
            }
        }

        //document represents one card so we need to include all stats defined 
        static public void InsertCard(string name, int attack, int defense, int mana, int maxMana, int crystalCost) {
            var stats = client.GetDatabase("cards").GetCollection<BsonDocument>("monsters");
            var doc = new BsonDocument
            {
                {"name", name},
                {"attack", attack},
                {"defense", defense},
                {"mana", mana },
                {"maxMana", maxMana },
                {"crystalCost", crystalCost }
            };
            stats.InsertOne(doc);
        }

       
        public static string CreatePlayer()
        {
            var collection = client.GetDatabase("Players").GetCollection<BsonDocument>("players");

            var player = new BsonDocument
            {
                { "Decks", new BsonArray{ } },
                { "Card_Instance", new BsonArray{ } }
            };

            collection.InsertOne(player);
            return player["_id"].ToString();
        }

        public static BsonDocument GetPlayer(string id)
        {
            var collection = client.GetDatabase("Players").GetCollection<BsonDocument>("players");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
            var player = collection.Find(filter).FirstOrDefault();
            
            return player;

        }

        public static void WriteToFile(string id)
        {
            var player = $"playerID:{id}";

            if(Directory.Exists(path))
            {

                File.WriteAllText(Path.Combine(path,"settings.dat"), player);
            }
            else
            {
                Directory.CreateDirectory(path);
                File.WriteAllText(Path.Combine(path, "settings.dat"), player);
            }
        }

        public static bool PathExists()
        {
            return Directory.Exists(path);
        }

        public static bool FileExists()
        {
            return File.Exists(Path.Combine(path, "settings.dat"));
        }

        public static string ReadFromFile()
        {
            if (Directory.Exists(path))
            {
                string text = System.IO.File.ReadAllText(Path.Combine(path, "settings.dat"));
                
                return text.Split(":")[1];
            }
            else
            {
                return "";
            }
        }

        public static async void AddCardToDeck(Card card, Deck deck)
        {
            var collection = client.GetDatabase("Players").GetCollection<BsonDocument>("players");
        }

        public static void CreateDeck(Player player, Deck deck)
        {
            var collection = client.GetDatabase("Players").GetCollection<BsonDocument>("players");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(player.id));

            var deckID = ObjectId.GenerateNewId();
            var deckData = new BsonDocument
            {
                { "_id", deckID},
                { "Card_Instance_Ids", new BsonArray{ } },
                { "Active", false}
            };

            var update = Builders<BsonDocument>.Update.Push("Decks", deckData);

            collection.FindOneAndUpdate(filter, update);

            deck.Id = deckID.ToString();
            Console.WriteLine(deck.Id);
        }

        public static void InsertCardIntoDeck(Player player, Deck deck, Card card)
        {

        }

        public static void RemoveCardFromDeck(Player player, Deck deck, Card card)
        {

        }

        public static List<Card> GetPlayerCards(Player player)
        {
            List<Card> cards = new List<Card>();
            return cards;
        }

        public static void AddCardToPlayerCards(Player player, Card card)
        {

        }

    }
}
