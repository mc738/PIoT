using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PIoT
{
    public class NodeLink
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }
    }
}
