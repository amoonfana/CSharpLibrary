using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleNet
{
    class Service
    {
        public delegate void OnAccept(ProtocolHandler protocolHandler);
        public delegate void OnConnect(ProtocolHandler protocolHandler);
        public delegate void OnReceive(ProtocolHandler protocolHandler);
        public delegate void OnSend(ProtocolHandler protocolHandler);

        public OnAccept onAccept;
        public OnConnect onConnect;
        public OnReceive onReceive;
        public OnSend onSend;

        public IAsyncResult BeginAccept(byte[] IP, int port, int maxConnection, AProtocol protocol)
        {
            IAsyncResult ar = null;

            try
            {
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint localEndPoint = new IPEndPoint(new IPAddress(IP), port);
                
                listener.Bind(localEndPoint);
                listener.Listen(maxConnection);

                Console.WriteLine("Waiting for a connection...\n");
                ProtocolHandler protocolHandler = new ProtocolHandler(protocol);
                protocolHandler.listener = listener;
                
                ar = listener.BeginAccept(new AsyncCallback(AcceptCallback), protocolHandler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return ar;
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            Console.WriteLine("A connection has come...\n");

            try
            {
                ProtocolHandler oldHandler = ar.AsyncState as ProtocolHandler;
                ProtocolHandler newHandler = new ProtocolHandler(oldHandler.protocol);

                newHandler.listener = oldHandler.listener;
                newHandler.communicator = newHandler.listener.EndAccept(ar);

                //Socket listener = ar.AsyncState as Socket;
                //Socket communicator = listener.EndAccept(ar);

                //protocolHandler protocolHandler = new protocolHandler(communicator);
                onAccept.Invoke(newHandler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public IAsyncResult BeginConnect(byte[] IP, int port, AProtocol protocol)
        {
            IAsyncResult ar = null;

            try
            {
                Socket connector = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint remoteEndPoint = new IPEndPoint(new IPAddress(IP), port);

                Console.WriteLine("Try to connect to {0}...\n", remoteEndPoint.ToString());
                ProtocolHandler protocolHandler = new ProtocolHandler(protocol);
                protocolHandler.communicator = connector;
                ar = connector.BeginConnect(remoteEndPoint, new AsyncCallback(ConnectCallback), protocolHandler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return ar;
        }

        public void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                ProtocolHandler protocolHandler = ar.AsyncState as ProtocolHandler;

                //Socket client = ar.AsyncState as Socket;
                
                protocolHandler.communicator.EndConnect(ar);
                Console.WriteLine("Connected to {0}.\n", protocolHandler.communicator.RemoteEndPoint.ToString());
                //protocolHandler protocolHandler = new protocolHandler(client);

                onConnect.Invoke(protocolHandler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void BeginReceive(ProtocolHandler protocolHandler)
        {
            try
            {
                protocolHandler.communicator.BeginReceive(protocolHandler.buffer, 0, protocolHandler.bufferSize, 0, new AsyncCallback(ReceiveCallback), protocolHandler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                ProtocolHandler protocolHandler = ar.AsyncState as ProtocolHandler;

                int bytesReceived = protocolHandler.communicator.EndReceive(ar);

                if (bytesReceived > 0)
                {
                    protocolHandler.protocol.deserialize(protocolHandler.buffer, bytesReceived);

                    for (int i = 0; i < protocolHandler.protocol.Count; ++i)
                    {
                        Console.WriteLine("{0}\n<- {1}\n", protocolHandler.protocol.toString(i), protocolHandler.communicator.RemoteEndPoint.ToString());
                        protocolHandler.protocol.parseProtocol(i);
                    }

                    onReceive.Invoke(protocolHandler);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void BeginSend(ProtocolHandler protocolHandler)
        {
            try
            {
                for (int i = protocolHandler.protocol.Count - 1; i >= 0; --i)
                {
                    byte[] byteData = protocolHandler.protocol.serialize(i);

                    protocolHandler.communicator.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), protocolHandler);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void SendCallback(IAsyncResult ar)
        {
            try
            {
                ProtocolHandler protocolHandler = ar.AsyncState as ProtocolHandler;

                int bytesSent = protocolHandler.communicator.EndSend(ar);

                if (!protocolHandler.communicator.Connected)
                {
                    Console.WriteLine("{0} is disconnected.\n", protocolHandler.communicator.RemoteEndPoint.ToString());
                    protocolHandler.communicator.Shutdown(SocketShutdown.Both);
                    protocolHandler.communicator.Close();
                    return;
                }

                Console.WriteLine(" -> {0}\n", protocolHandler.communicator.RemoteEndPoint.ToString());

                onSend.Invoke(protocolHandler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
