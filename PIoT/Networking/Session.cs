using PIoT.Events;
using PIoT.Messaging;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PIoT.Networking
{
    public class Session
    {
        private readonly Guid id;
        private readonly Guid linkId;
        private readonly ConnectionHandler handler;
        private readonly IncomingChannel incomingChannel;
        private readonly OutgoingChannel outgoingChannel;
        private readonly bool readOnly;

        private bool running;

        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        public bool Active => handler.Active;

        public bool Running => running;

        

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="incomingChannel"></param>
        /// <param name="outgoingChannel"></param>
        public Session(Guid id, Guid linkId, ConnectionHandler handler, IncomingChannel incomingChannel, OutgoingChannel outgoingChannel, bool readOnly = false)
        {
            this.id = id;
            this.linkId = linkId;
            this.handler = handler;
            this.incomingChannel = incomingChannel;
            this.outgoingChannel = outgoingChannel;
            this.readOnly = readOnly;
        }

        /// <summary>
        /// Dispatch the session to a background thread and activate it.
        /// </summary>
        public void Dispatch(bool inheritState = true)
        {
            // If the connection is active and the session is currently not running start it running.
            if (Active && !Running)
            {
                Task.Run(() => Run(inheritState));
            }
        }

        private void Run(bool inheritState = true)
        {
            // IMPORTANT - running should not be touched be any other method in this class.
            // It represents if this is running on a background thread.
            running = true;
            // Set active to true, the only way to set this to false should be by called Kill()

            // If inheritState == true, state be handling any outstanding actions.
            if (inheritState)
            {
                // TODO negatio 'Shared State' - i.e. both sides deal with any outstanding actions.
            }

            // TODO add run modes

            // If the session is read only there is no need to handle outgoing messages.
            if (!readOnly)
                Task.Run(() => HandleOutgoing());

            // Loop while the session is active. Hopefully do/while works for this but testing needed.
            do
            {
                // Wait for a message to come in
                var data = handler.Read();

                // Incoming...
                var message = new Message(Encoding.UTF8.GetString(data));

                // TODO add choice, handle in session (i.e. for redirects, responses etc.) or handle by node.
                // Specific this in message header or based on message type?
                // Raise the message recieved event.
                HandleMessageRecieved(message);

            } while (Active);

            // TODO add cleanup

            // This is the only place that should set running to false again.
            // Also it is important that this is the last action in the method.
            running = false;
        }

        /// <summary>
        /// Add a message to the outgoing channel for the session,
        /// or send directly.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="addToQueue">If true, add the message to the outgoing queue. If false, attempt to send directly.</param>
        public void AddMessage(Message message, bool addToQueue = true)
        {
            // If the session is not active, or reaad only silently discard and log.
            if (!Active || readOnly)
            {
                // TODO log error "Connection is not active or read only. Connection Details: linkId: {}, session id: {}, active: {}, readonly: {}"
                return;
            }

            if (addToQueue)
                outgoingChannel.AddMessage(message);
            else
                handler.Write(message.ToBytes());
        }

        /// <summary>
        /// Kill the session by closing the connection handled. Any background threads will be stopped and clean up/shut down run.
        /// Under the hood this calls .Close() on the sessions connection handler.
        /// </summary>
        public void Kill()
        {
            handler.Close();
        }

        protected virtual void HandleMessageRecieved(Message message) => MessageRecieved?.Invoke(this, new MessageRecievedEventArgs(id, linkId, message));

        private void HandleOutgoing()
        {
            do
            {
                while (!outgoingChannel.HasMessages) ;

                var message = outgoingChannel.NextMessage();

                handler.Write(message.ToBytes());
            } while (Active);

            // TODO add cleanup
        }
    }
}
