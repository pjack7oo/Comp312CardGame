using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Crystal_Wars.Source.Core
{
    class Database
    {
        private static MongoClient client = new MongoClient("mongodb+srv://root:comp312@cluster0.kjv42.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
        private static readonly string path = System.IO.Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.Personal), "CrystalWars");
        public void DBConnectionTest()
        {
            var client = new MongoClient("mongodb+srv://root:comp312@cluster0.kjv42.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
            var dbList = client.ListDatabases().ToList();

            Console.WriteLine("The list of databases are:");
            foreach (var item in dbList)
            {
                Console.WriteLine(item);
            }
        }

        public void RetrieveDocuments()
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
        public void InsertCard(string name, int attack, int defense, int mana, int maxMana, int crystalCost) {
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
                { "Decks", new BsonArray{ } }
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

    }
}
