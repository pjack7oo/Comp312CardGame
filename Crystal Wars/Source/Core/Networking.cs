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
        public static TcpClient tcpClient;
        public static TcpClient client;
        public static TcpListener server;
        public static IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        public static NetworkStream netStream;

        public static Match match;
        public Networking()
        {
            netStream = tcpClient.GetStream();
        }


        public static void Connect(String server, String message)
        {
            try
            {
                tcpClient = new TcpClient(server, port);

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                Message hello = Serialize(message);

                NetworkStream netStream = tcpClient.GetStream();

                netStream.Write(hello.Data, 0, hello.Data.Length);

                Console.WriteLine("Sent: {0}", message);

                data = new Byte[256];

                String responseData = String.Empty;

                Int32 bytes = netStream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                //netStream.Close();
                //tcpClient.Close();
                //EndConnection();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        

        public static void EndClientConnection()
        {
            try
            {
                //netStream.Close();
                tcpClient.Close();

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



        public static void Start()
        {
            server = null;
            try
            {
                server = new TcpListener(localAddr, port);
                server.Start();

                
                String data = null;

                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    NetworkStream stream = client.GetStream();
                    
                    int i;

                    while (true)
                    {
                        
                        //var obj = Deserialize(message);
                        data =
                        //data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        //Console.WriteLine("Recieved: {0}", data);

                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
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

            Console.WriteLine("\nHit eneter to contunue...");
            Console.Read();
        }

        public static void SendData(Message message)
        {
            NetworkStream netStream = tcpClient.GetStream();

            netStream.Write(message.Data, 0, message.Data.Length);
            //netStream.Write();
        }

        public static Message ReadData()
        {
            NetworkStream stream = client.GetStream();
            byte[] myReadBuffer = new byte[1024];
            var bufferSpan = new Span<byte>(myReadBuffer);
            return new Message();
        }


        public static Message Serialize(object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(memoryStream, obj);
                return new Message { Data = memoryStream.ToArray() };
            }
        }
        public static object Deserialize(Message message)
        {
            using (var memoryStream = new MemoryStream(message.Data))
                return (new BinaryFormatter()).Deserialize(memoryStream);
        }

    }

    public class Message
    {
        public byte[] Data { get; set; }
    }
}
