// <copyright file="Message.cs" company="mclifford.dev">
// Copyright (c) mclifford.dev. All rights reserved.
// </copyright>

using System.Text;

namespace PIoT.Messaging
{
    /// <summary>
    /// A base PIoT message.
    /// </summary>
    public class Message
    {
        private readonly string body;

        private MessageDeliveryStatus status = MessageDeliveryStatus.Created;

        public MessageDeliveryStatus Status => status;

        public Message(string body)
        {
            this.body = body;
        }

        public string Body => body;

        public void MarkAsSending()
        {
            status = MessageDeliveryStatus.Sending;
        }

        public void MarkAsSent()
        {
            if (status == MessageDeliveryStatus.Sent)
            {
                status = MessageDeliveryStatus.Sent;
            }
            else
            {
                // Handle if the status is not right
            }
        }

        public void MarkAsAwaitingDeliveryConfirmation()
        {
            if (status == MessageDeliveryStatus.Sent)
            {
                status = MessageDeliveryStatus.AwaitingDeliveryConfirmation;
            }
            else
            {
                // Handle if the status is not right
            }
        }

        public void MarkAsDelivered()
        {
            if (status == MessageDeliveryStatus.AwaitingDeliveryConfirmation)
            {
                status = MessageDeliveryStatus.Delivered;
            }
            else
            {
                // Handle if the status is not right
            }
        }

        public void MarkAsDiscarded()
        {
            status = MessageDeliveryStatus.Discarded;
        }

        public byte[] ToBytes() => Encoding.UTF8.GetBytes(body);
    }
}
