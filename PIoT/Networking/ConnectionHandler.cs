using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PIoT.Networking
{
    public class ConnectionHandler
    {
        private readonly TcpClient client;
        private readonly NetworkStream stream;

        public bool Active => client.Connected;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionHandler"/> class.
        /// </summary>
        /// <param name="client"></param>
        public ConnectionHandler(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();
            
        }

        public byte[] Read()
        {
            // Block until data is available
            // TODO add a timeout
            while (!stream.DataAvailable) ;

            byte[] bytes = new byte[client.Available];
            //string s = Encoding.UTF8.GetString(bytes);
            //Console.WriteLine(s);

            stream.Read(bytes, 0, client.Available);

            return bytes;
        }

        public void Write(byte[] data) => stream.Write(data, 0, data.Length);

        public void Close()
        {
            // TODO add send close message.
            client.Close();
        }
    }
}
