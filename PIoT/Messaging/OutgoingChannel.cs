using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace PIoT.Messaging
{
    public class OutgoingChannel
    {
        private readonly Queue<Message> queue = new Queue<Message>();

        private Message currentMessage;

        public bool HasMessages => queue.Count > 0;

        public OutgoingChannel()
        {
        }

        public Message NextMessage() => queue.Dequeue();

        public void AddMessage(Message message) =>
            queue.Enqueue(message);

        public void HandleNextMessage()
        {
            if (currentMessage == null)
            {
                currentMessage = queue.Dequeue();

                // Send the message
                currentMessage.MarkAsSending();

                currentMessage.MarkAsSent();

                // Await delivery confirmation
                currentMessage.MarkAsAwaitingDeliveryConfirmation();

                // Once delivery complete.
                currentMessage.MarkAsDelivered();

                currentMessage = null;
            }
            else
            {
                // A message is still await completion.
            }
        }
    }
}
