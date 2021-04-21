using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CompCardGame.Source.Core
{
    class Networking
    {
        private static Int32 port = 13000;
        public static TcpClient tcpClient;
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


                NetworkStream netStream = tcpClient.GetStream();

                netStream.Write(data, 0, data.Length);

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

                Byte[] bytes = new Byte[256];
                String data = null;

                while(true)
                {
                    Console.Write("Waiting for a connection... ");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    NetworkStream stream = client.GetStream();

                    int i;

                    while((i= stream.Read(bytes, 0, bytes.Length))!=0)
                    {
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Recieved: {0}", data);

                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    //client.Close();
                }
            }
            catch(SocketException e)
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

        
    }
}
