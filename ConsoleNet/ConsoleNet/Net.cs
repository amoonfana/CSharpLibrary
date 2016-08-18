using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNet
{
    class Net
    {
        public static void Main(String[] args)
        {
            byte[] IP = new byte[] { 127, 0, 0, 1 };
            int port = 65535;
            int maxConnection = 64;

            ChatServer server = new ChatServer();
            ChatClient client = new ChatClient();
            string command = null;

            Console.WriteLine("Input 's' to start server or 'c' to start client");
            command = Console.ReadLine();

            if (command == "s")
            {
                server.service.BeginAccept(IP, port, maxConnection, new AddProtocol());
            }

            if (command == "c")
            {
                client.service.BeginConnect(IP, port, new AddProtocol());
            }

            while (true) { }
        }
    }
}
