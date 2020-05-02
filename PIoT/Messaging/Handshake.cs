using System;
using System.Collections.Generic;
using System.Text;

namespace PIoT.Messaging
{
    public class Handshake
    {
        private readonly Guid from;
        private readonly bool valid;
        
        public Guid From => from;

        public bool Valid => valid;

        private Handshake(Guid from, bool valid)
        {
            this.from = from;
            this.valid = valid;
        }

        public static Handshake CreateFromBytes(byte[] data)
        {
            var id = new Guid(data);

            return new Handshake(id, true);
        }

        public static Handshake Create(Guid id)
        {
            return new Handshake(id, true);
        }

        public byte[] ToBytes()
        {
            return from.ToByteArray();
        }
    }
}
