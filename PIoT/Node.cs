using PIoT.Events;
using PIoT.Logging;
using PIoT.Messaging;
using PIoT.Networking;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PIoT
{
    public class Node
    {
        private readonly Dictionary<Guid, Link> links = new Dictionary<Guid, Link>();
        private readonly Listener listener;
        private readonly Guid id;
        private readonly string name;

        private readonly IPAddress address;
        private readonly int port;
        private readonly bool log;

        private Node(Guid id, string name, TcpListener tcpListener, IPAddress address, int port, bool log = true)
        {
            listener = new Listener(tcpListener);
            listener.ConnectionRecieved += OnConnection;
            this.id = id;
            this.name = name;
            this.address = address;
            this.port = port;
            this.log = log;
        }

        /// <summary>
        /// Create a new node.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static Node Create(NodeSettings settings, bool log = true)
        {
            var address = IPAddress.Parse(settings.Address);

            var tcpListener = new TcpListener(address, settings.Port);

            var node = new Node(settings.Id, settings.Name, tcpListener, address, settings.Port, log);

            settings.Links.ForEach(nl => node.AddLink(Link.CreateFromNodeLink(nl)));

            node.Log(new LogItem("Node", "Successfully Created", "The node was successfully created.", LogItemTypes.Success));

            node.Listen();

            return node;
        }

        private void Listen()
        {
            Log(new LogItem("Node", "Listener Starting", $"The node is attempting to start listenering on {address}:{port}", LogItemTypes.Information));

            if (!listener.Listening)
                Task.Run(() => listener.Listen());

            Log(new LogItem("Node", "Listener Started", $"The node has started listener on {address}:{port}", LogItemTypes.Success));
        }

        public void Connect(Guid linkId)
        {
            Log(new LogItem("Node", "Starting Connection", $"Attempting to connection to {linkId}", LogItemTypes.Information));

            if (links.ContainsKey(linkId))
            {
                Log(new LogItem("Node", "Link Found", $"Link has been found, attempting connection.", LogItemTypes.Success));

                var link = links[linkId];

                TcpClient client = new TcpClient();

                client.Connect(link.Settings.Address, link.Settings.Port);

                var connectionHandler = new ConnectionHandler(client);

                Log(new LogItem("Node", "Connection Estiablished", $"The connection was successfully estiablished. Sending handshake.", LogItemTypes.Success));

                // Attemp a connection and send handshake.
                var handshake = Handshake.Create(id);

                connectionHandler.Write(handshake.ToBytes());

                var sessionIdBytes = connectionHandler.Read();

                var sessionId = new Guid(sessionIdBytes);

                Log(new LogItem("Node", "Session Id Recieved", $"The session id ({sessionId}) was recieved.", LogItemTypes.Success));

                link.InitializeSession(sessionId, connectionHandler, Session_MessageRecieved);

                Log(new LogItem("Node", "Session Initialzed", $"A new session (id: {sessionId}) has been initalized.", LogItemTypes.Success));
            }
            else
            {
                // TODO handle missing link (lol).
            }
        }


        public void SendMessage(Guid to, Message message)
        {
            Log(new LogItem("Node", "Message Sending", $"Attempting to send message to {to}.", LogItemTypes.Information));

            if (links.ContainsKey(to))
            {


                links[to].AddMessage(message);

                Log(new LogItem("Node", "Message Sent", $"Message sent to {to}.", LogItemTypes.Success));
            }
        }


        protected virtual void OnConnection(object sender, ConnectionRecievedEventArgs e)
        {
            // If eventargs are null of the connection is dead do nothing.
            if (e == null || !e.Client.Connected)
            {
                Log(new LogItem("Node", "Invalid Connection", "No further actions taken.", LogItemTypes.Warning));

                return;
            }

            Log(new LogItem("Node", "Connection Recieved", $"The node recieved a new connection.", LogItemTypes.Information));

            // Create a new connection handler and get the handshake.
            var connectionHandler = new ConnectionHandler(e.Client);

            var handshakeBytes = connectionHandler.Read();

            var handshake = Handshake.CreateFromBytes(handshakeBytes);

            if (handshake.Valid)
            {

                Log(new LogItem("Node", "Valid Handshake", $"The handshake is valid.", LogItemTypes.Success));

                Link link;

                if (links.ContainsKey(handshake.From))
                    link = links[handshake.From];
                else
                    link = CreateNewLink(connectionHandler, handshake);

                // Send the session id as a confirmation.
                var sessionId = Guid.NewGuid();

                connectionHandler.Write(sessionId.ToByteArray());

                // Initalize a session on the link and add the message recevied event handler for the node.
                link.InitializeSession(sessionId, connectionHandler, Session_MessageRecieved);

                Log(new LogItem("Node", "Session Initialzed", $"A new session (id: {sessionId}) has been initalized.", LogItemTypes.Success));

            }
            else
            {
                // TODO Handle invalid handshake/invalid connection.
                Log(new LogItem("Node", "Invalid Handshake/Connnect", $"A handshake was invalid or the connect has troubles. No further actions taken.", LogItemTypes.Warning));
            }
        }

        private void Session_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {

            Log(new LogItem("Node", "Message Received", e.Message.Body, LogItemTypes.Information));
        }

        private void OnLog(object sender, LogEventArgs e)
        {
            Log(e.LogItem);
        }

        private void Log(LogItem item)
        {
            if (log)
            {
                switch (item.Type)
                {
                    case LogItemTypes.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("[ERROR]  ");
                        break;
                    case LogItemTypes.Information:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("[INFO]    ");

                        break;
                    case LogItemTypes.Success:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[SUCCESS] ");
                        break;
                    case LogItemTypes.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("[WARN]    ");
                        break;
                    default:
                        break;
                }

                Console.Write('\t');
                Console.ResetColor();

                Console.Write($"{item.From}({name})@{item.Time}: {item.Summary}{Environment.NewLine}");
                Console.WriteLine($"\tDetails: {item.Details}");
            }
        }

        private Link CreateNewLink(ConnectionHandler connectionHanlder, Handshake handshake)
        {
            throw new NotImplementedException();
            // Request details from the connected node to create the link.
            //var settings = new LinkSettings(handshake.From);

            //var link = new Link(new Link(settings));

            //AddLink(link);

            //return link;
        }

        private void AddLink(Link link)
        {
            if (!links.ContainsKey(link.Id))
                links.Add(link.Id, link);
        }
    }
}
