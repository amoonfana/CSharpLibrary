using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleNet
{
    class ChatClient
    {
        public Service service;

        public ChatClient()
        {
            service = new Service();

            service.onAccept += OnAccept;
            service.onConnect += OnConnect;
            service.onReceive += OnReceive;
            service.onSend += OnSend;
        }

        void OnAccept(ProtocolHandler protocolHandler)
        {
        }

        void OnConnect(ProtocolHandler protocolHandler)
        {
            Console.WriteLine("You may start to chat.");

            service.BeginReceive(protocolHandler);
            while (true)
            {
                string input = Console.ReadLine();
                protocolHandler.protocol.addData(input);
                service.BeginSend(protocolHandler);
            }
        }

        void OnReceive(ProtocolHandler protocolHandler)
        {
            protocolHandler.communicator.BeginReceive(protocolHandler.buffer, 0, protocolHandler.bufferSize, 0, new AsyncCallback(service.ReceiveCallback), protocolHandler);
        }

        void OnSend(ProtocolHandler protocolHandler)
        {
        }
    }
}
