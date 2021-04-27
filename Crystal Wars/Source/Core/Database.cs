﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Crystal_Wars.Source.Core
{
    class Database
    {
        private static MongoClient client = new MongoClient("mongodb+srv://root:comp312@cluster0.kjv42.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");

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

       
    }
}
