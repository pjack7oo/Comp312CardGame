using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;


namespace Crystal_Wars.Source.Core
{
    class Networking
    {
        private static Int32 port = 13000;
        public static TcpClient tcpClient = null;
        public static TcpClient client = null;
        public static TcpListener server;
        public static IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        public static NetworkStream netStream;

        public static Match match;
        public Networking()
        {
            netStream = tcpClient.GetStream();
        }


        public async static void Connect(String server)
        {
            try
            {

                tcpClient = new TcpClient(server, port);
                
                
                
                
                if (tcpClient.Connected)
                {
                    netStream = tcpClient.GetStream();
                    Networking.SendData(JMessage.Serialize(JMessage.FromValue(Game.match.GetPlayer(0))));
                    //var msg = Serialize("Hello");
                    //Console.WriteLine(msg.Data.Length);
                    //var buffer = new ReadOnlyMemory<byte>(msg.Data);

                    //netStream.WriteAsync(buffer);


                    //var obj = ReadData();
                    //ClientUpdate(obj);


                }
                
                

                
                
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            
        }

        

        public static void EndClientConnection()
        {
            try
            {
                //netStream.Close();
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }
                

            }
            catch (ArgumentException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }

        }
        public static void EndSeverConnection()
        {
            try
            {
                //netStream.Close();
                server.Stop();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }

        }



        public static async void Start()
        {
            server = null;
            try
            {
                server = new TcpListener(localAddr, port);
                server.Start();
                
                
                //String data = null;

                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    client = server.AcceptTcpClient();
                    

                    if (client.Connected)
                    {
                        Console.WriteLine("Connected!");
                        netStream = client.GetStream();
                        Networking.SendData(JMessage.Serialize(JMessage.FromValue(Game.match.GetPlayer(0))));
                        break;
                        //byte[] myReadBuffer = new byte[1024];
                        //var bufferSpan = new Memory<byte>(myReadBuffer);
                        //int read = await netStream.ReadAsync(bufferSpan);
                        //if (read > 0)
                        //{
                        //    //var obj = Deserialize(new Message() { Data = bufferSpan.ToArray() });
                        //    //var data = System.Text.Encoding.ASCII.GetString(bufferSpan.ToArray(), 0, bufferSpan.Length);
                        //    //Console.WriteLine("Received: {0}", obj);
                        //    //SendData(obj);
                        //    //var obj = Deserialize(new Message() { Data = bufferSpan.ToArray() });
                        //    break;
                        //    //Console.WriteLine(obj);
                        //}
                    }

                    
                    
                    

                   

                    //client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.Write("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            
        }

        public async static void Update()
        {
            if (client != null)
            {
                while (client.Connected)
                {
                    try
                    {
                        byte[] myReadBuffer = new byte[1024];
                        var bufferSpan = new Memory<byte>(myReadBuffer);
                        int read = await netStream.ReadAsync(bufferSpan);
                        if (read > 0)
                        {

                            //var obj = Deserialize(new Message() { Data = bufferSpan.ToArray() });
                            var data = System.Text.Encoding.ASCII.GetString(bufferSpan.ToArray());
                            Console.WriteLine("Received: {0}", data);
                            JMessage message = JMessage.Deserialize(data);
                            if (message.Type == typeof(Player))
                            {
                                Game.match.AddSecondPlayer(message.Value.ToObject<Player>());
                            }
                            //var bj = Deserialize(new Message() { Data = bufferSpan.ToArray() });

                            //Console.WriteLine(obj);
                        }
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e);
                    }
                    


                }
            }
            
        }
        public async static void ClientUpdate()
        {
            if (tcpClient != null)
            {
                while (tcpClient.Connected)
                {
                    try
                    {
                        byte[] myReadBuffer = new byte[1024];
                        var bufferSpan = new Memory<byte>(myReadBuffer);
                        int read = await netStream.ReadAsync(bufferSpan);
                        if (read > 0)
                        {
                            var data = System.Text.Encoding.ASCII.GetString(bufferSpan.ToArray());
                            //var obj = Deserialize(new Message() { Data = bufferSpan.ToArray() });
                            Console.WriteLine(data);
                            JMessage message = JMessage.Deserialize(data);
                            if (message.Type == typeof(Player))
                            {
                                Game.match.AddSecondPlayer(message.Value.ToObject<Player>());
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e);
                    }
                    
                }
            }
            
        }

        public static void SendData(string obj)
        {
            //netStream = tcpClient.GetStream();
            //var msg = JMessage.Serialize(JMessage.FromValue(obj));
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(obj);
            var bufferSpan = new ReadOnlyMemory<byte>(data);
            netStream.WriteAsync(bufferSpan);
            //netStream.Write();
        }

        public  static JMessage ReadData()
        {
            
            byte[] myReadBuffer = new byte[1024];
            var bufferSpan = new Span<byte>(myReadBuffer);
            netStream.Read(bufferSpan);
            JMessage message = JMessage.Deserialize(bufferSpan.ToString());


            return message;
        }


        //public static Message Serialize(object obj)
        //{
        //    //using (var memoryStream = new MemoryStream())
        //    //{
        //    //    (new BinaryFormatter()).Serialize(memoryStream, obj);
        //    //    return new Message { Data = memoryStream.ToArray() };
        //    //}

        //}
        //public static object Deserialize(Message message)
        //{
        //    //using (var memoryStream = new MemoryStream(message.Data))
        //    //    return (new BinaryFormatter()).Deserialize(memoryStream);
        //}

    }

    public class Message
    {
        public byte[] Data { get; set; }
    }

    class JMessage
    {
        public Type Type { get; set; }
        public JToken Value { get; set; }

        public static JMessage FromValue<T>(T value)
        {
            return new JMessage { Type = typeof(T), Value = JToken.FromObject(value) };
        }

        public static string Serialize(JMessage message)
        {
            return JToken.FromObject(message).ToString();
        }

        public static JMessage Deserialize(string data)
        {
            return JToken.Parse(data).ToObject<JMessage>();
        }
    }
}
