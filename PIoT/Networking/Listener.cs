using PIoT.Events;
using PIoT.Messaging;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PIoT.Networking
{
    public class Listener
    {
        private bool run;
        private readonly TcpListener listener;

        public bool Listening => run;

        public Listener(TcpListener listener)
        {
            this.listener = listener;
        }

        public event EventHandler<ConnectionRecievedEventArgs> ConnectionRecieved;

        public bool Running => run;

        public void Listen()
        {
            run = true;

            listener.Start();

            // Start the listening loop, and pass connections back to the node for handling.
            do
            {
                var connection = listener.AcceptTcpClient();

                HandleConnectionRecieved(connection);

            } while (run);

            listener.Stop();
        }

        protected virtual void HandleConnectionRecieved(TcpClient client) => ConnectionRecieved?.Invoke(this, new ConnectionRecievedEventArgs(client));
    }
}
