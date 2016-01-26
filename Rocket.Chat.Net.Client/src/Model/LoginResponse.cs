using System;
using Net.DDP.Client;
using Newtonsoft.Json;

namespace Rocket.Chat.Net.Client
{
    public class LoginResponse : DDPBaseModel
    {
        [JsonProperty ("id")]
        public string Id { get; set; }

        [JsonProperty ("token")]
        public string Token { get; set; }

        [JsonProperty ("tokenExpires")]
        public EJsonDate TokenExpiration { get; set; }

        public bool HasToken => !string.IsNullOrWhiteSpace (Token);

        // To satisfy Util.FromDDPMessageResult generic restriction
        public LoginResponse () {}

        public LoginResponse (DDPMessageData ddpMessageData = null)
        {
            DDPMessageData = ddpMessageData;
        }
    }
}

