using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace PIoT.Networking
{
    public class LinkSettings
    {
        private readonly Guid id;
        private readonly IPAddress address;
        private readonly int port;

        public LinkSettings(Guid id, IPAddress address, int port)
        {
            this.id = id;
            this.address = address;
            this.port = port;
        }

        public Guid Id => id;

        public IPAddress Address => address;

        public int Port => port;
    }
}
