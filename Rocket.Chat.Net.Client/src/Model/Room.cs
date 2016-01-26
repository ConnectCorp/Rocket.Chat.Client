using System;
using Newtonsoft.Json;

namespace Rocket.Chat.Net.Client
{
    public class Room : DDPBaseModel
    {
        [JsonProperty ("open")]
        public bool Open { get; set; }

        [JsonProperty ("alert")]
        public bool Alert { get; set; }

        [JsonProperty ("unread")]
        public int Unread { get; set; }

        [JsonProperty ("ts")]
        public EJsonDate Ts { get; set; }

        [JsonProperty ("rid")]
        public string Rid { get; set; }

        [JsonProperty ("name")]
        public string Name { get; set; }

        [JsonProperty ("t")]
        public string Type { get; set; }

        [JsonProperty ("ls")]
        public EJsonDate Ls { get; set; }

        public Room ()
        {
        }
    }
}

