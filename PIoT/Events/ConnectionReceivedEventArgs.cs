using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace PIoT.Events
{
    public class ConnectionRecievedEventArgs
       : EventArgs
    {
        private readonly TcpClient client;

        public TcpClient Client => client;

        public ConnectionRecievedEventArgs(TcpClient client)
        {
            this.client = client;
        }
    }
}
