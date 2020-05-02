using PIoT.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIoT.Events
{
    public class MessageRecievedEventArgs
        : EventArgs
    {
        private readonly Guid sessionId;
        private readonly Guid linkId;
        private readonly Message message;

        public Guid SessionId => sessionId;

        public Guid LinkId => linkId;

        public Message Message => message;

        public MessageRecievedEventArgs(Guid sessionId, Guid linkId, Message message)
        {
            this.sessionId = sessionId;
            this.linkId = linkId;
            this.message = message;
        }
    }
}
