// <copyright file="Link.cs" company="mclifford.dev">
// Copyright (c) mclifford.dev. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using PIoT.Events;
using PIoT.Messaging;

namespace PIoT.Networking
{
    /// <summary>
    /// A class representing a link between two nodes. The link has the id of the connected node.
    /// </summary>
    public class Link
    {
        private readonly Guid id;
        private readonly LinkSettings settings;
        private readonly Queue<Message> holdingQueue = new Queue<Message>();

        private Session session;

        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// </summary>
        /// <param name="settings">The link settings.</param>
        /// <exception cref="ArgumentNullException">Thrown is settings is null.</exception>
        public Link(LinkSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            id = settings.Id;
            this.settings = settings;
        }

        public static Link CreateFromNodeLink(NodeLink nodeLink)
        {
            //TODO this is needs to be cleaned up.
            return new Link(new LinkSettings(nodeLink.Id, IPAddress.Parse(nodeLink.Address), nodeLink.Port));
        }

        /// <summary>
        /// Gets the link id.
        /// </summary>
        public Guid Id => id;

        /// <summary>
        /// Gets a value indicating whether the link has messages in holding.
        /// </summary>
        public bool HasMessagesInHolding => holdingQueue.Count > 0;

        /// <summary>
        /// Gets the link settings.
        /// </summary>
        public LinkSettings Settings => settings;

        /// <summary>
        /// Gets a value indicating whether the link currently has an active session.
        /// </summary>
        public bool Active => session != null;

        /// <summary>
        /// Initialize a new session for the link.
        /// This will create a session and dispatch it to a background thread for processing.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="connectionHandler">The connection handler for the session.</param>
        /// <param name="messageRecievendEcentHandler">The messaged recieved event handler.</param>
        /// <param name="inheritState">If true (default), inherit state from pervious sessions. If false, the session will be a clean slate.</param>
        public void InitializeSession(Guid sessionId, ConnectionHandler connectionHandler, EventHandler<MessageRecievedEventArgs> messageRecievendEcentHandler, bool inheritState = true)
        {
            if (connectionHandler == null || !connectionHandler.Active)
            {
                // TODO handle invalid connection.
                return;
            }

            // Create the incoming and outgoing channels from the data stored on the link.
            var incomingChannel = CreateIncomingChannel();
            var outgoingChannel = CreateOutgoingChannel();

            var session = new Session(sessionId, Id, connectionHandler, incomingChannel, outgoingChannel);

            // Plug up events, set the links session to the new session, pass inheritState and dispatch.
            // The session will handle setting up the state.
            session.MessageRecieved += messageRecievendEcentHandler;

            this.session = session;
            this.session.Dispatch(inheritState);
        }

        /// <summary>
        /// End the current session.
        /// </summary>
        public void EndSession()
        {
            if (session != null)
            {
                session.Kill();
                session = null;
            }
        }

        /// <summary>
        /// Add a message to the link.
        /// If the link has a active session it will be passed to that,
        /// if not it will be placed in holding for when the link is active.
        /// </summary>
        /// <param name="message">The message to be passed.</param>
        public void AddMessage(Message message)
        {
            if (Active)
                session.AddMessage(message);
            else
                holdingQueue.Enqueue(message);

        }

        private IncomingChannel CreateIncomingChannel()
        {
            var channel = new IncomingChannel();

            return channel;
        }

        private OutgoingChannel CreateOutgoingChannel()
        {
            var channel = new OutgoingChannel();

            return channel;
        }
    }
}
