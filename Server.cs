using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MCServer
{
    class Program
    {
        public static int EntityIdCounter = 0;
        private static ManualResetEvent allDone = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 25565);
            Socket socket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            socket.Bind(endPoint);
            socket.Listen(100);
            while (true)
            {
                allDone.Reset();
                Console.WriteLine("Waiting for conn");
                socket.BeginAccept(ar =>
                {
                    allDone.Set();
                    Client client = new Client(new NetworkStream(socket.EndAccept(ar)));
                    client.Handle();
                }, socket);
                allDone.WaitOne();
            }
        }
    }
}