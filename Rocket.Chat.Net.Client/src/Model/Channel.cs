using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rocket.Chat.Net.Client
{
    public class Channel
    {

        [JsonProperty ("_id")]
        public string Id { get; set; }

        [JsonProperty ("ts")]
        public EJsonDate Timestamp { get; set; }

        // Go figure what this is...
        [JsonProperty ("t")]
        public string T { get; set; }

        [JsonProperty ("name")]
        public string Name { get; set; }

        [JsonProperty ("usernames")]
        public List<string> Usernames { get; set; }

        [JsonProperty ("msgs")]
        public int MessageCount { get; set; }

        [JsonProperty ("default")]
        public bool Default { get; set; }

        [JsonProperty ("lm")]
        public EJsonDate Lm { get; set; }

        public Channel ()
        {
        }
    }
}

