using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace Crystal_Wars.Source.Core
{
    static class FileHandler
    {
        private static Hashtable Hashtable = new Hashtable();

        private static void LoadImage(string address)
        {
            if (address.Contains("Texture"))
            {
                var texture = new Texture(address)
                {
                    Smooth = true
                };

                StoreItem(GetName(address), texture);
            }
            else if (address.Contains("Sprite") || address.Contains("Icon"))
            {
                var sprite = new Sprite(new Texture(address) { Smooth = true });
                StoreItem(GetName(address), sprite);
            }
            
        }

        private static string GetName(string address)
        {
            var strings = address.Split('\\');

            var item = strings[strings.Length - 1].Split('.');
            return item[0];
        }
        private static void StoreItem<T>(string key, T item)
        {
            Hashtable.Add(key, item);
            //foreach (DictionaryEntry entry in Hashtable) {
            //    Console.WriteLine("{0}, {1}", entry.Key, entry.Value);
            //}
        }

        private static void SearchDirectory(string dir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(dir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        //Console.WriteLine(f);
                        LoadImage(f);
                    }
                    SearchDirectory(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        public static void LoadAllImages()
        {
            SearchDirectory("./Media");
        }


        public static object GetItem(string key) {
            return Hashtable[key];
        }
    }
}
