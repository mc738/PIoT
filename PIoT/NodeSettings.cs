using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PIoT
{
    public class NodeSettings
    {

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("links")]
        public List<NodeLink> Links { get; set; } = new List<NodeLink>();

        public static NodeSettings Load(string path)
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);

                // TODO add validation
                return JsonSerializer.Deserialize<NodeSettings>(json);
            }

            return null;
        }

        public void Save()
        {

        }
    }
}
