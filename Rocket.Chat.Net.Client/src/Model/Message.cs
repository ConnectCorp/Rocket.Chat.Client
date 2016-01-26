using System;
using Newtonsoft.Json;

namespace Rocket.Chat.Net.Client
{
    public class Message : DDPBaseModel
    {
        [JsonProperty ("ts")]
        public EJsonDate TokenExpiration { get; set; }

        [JsonProperty ("msg")]
        public string MessageContent { get; set; }

        [JsonProperty ("rid")]
        public string RoomId { get; set; }

        [JsonProperty ("u")]
        public UserDataBundle UserData { get; set; }

        [JsonProperty ("_id")]
        public string Id { get; set; }

        // To satisfy Util.FromDDPMessageResult generic restriction
        public Message ()
        {
        }

        public class UserDataBundle
        {
            [JsonProperty ("_id")]
            public string Id { get; set; }

            [JsonProperty ("username")]
            public string Username { get; set; }
        }

    }
}

