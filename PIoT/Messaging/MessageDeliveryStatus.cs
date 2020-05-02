using System;
using System.Collections.Generic;
using System.Text;

namespace PIoT.Messaging
{
    public enum MessageDeliveryStatus
    {
        Error = -1,
        Created = 0,
        Sending = 1,
        Sent = 2,
        AwaitingDeliveryConfirmation = 3,
        Delivered = 4,
        Discarded = 5,
        Holding = 6,
    }
}
