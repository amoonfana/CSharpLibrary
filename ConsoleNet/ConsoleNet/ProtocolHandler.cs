using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleNet
{
    public class ProtocolHandler
    {
        public Socket listener;
        public Socket communicator;
        public int bufferSize;
        public byte[] buffer;
        public AProtocol protocol;

        public ProtocolHandler(AProtocol _protocol)
        {
            listener = null;
            communicator = null;
            bufferSize = 1024;
            buffer = new byte[bufferSize];
            protocol = _protocol;
        }
    }
}
