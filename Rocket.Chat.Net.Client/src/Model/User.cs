using System;
using Net.DDP.Client;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rocket.Chat.Net.Client
{
    public class User : DDPBaseModel
    {
        [JsonProperty ("username")]
        public string Username { get; set; }

        [JsonProperty ("name")]
        public string Name { get; set; }

        [JsonProperty ("status")]
        public string Status { get; set; }

        [JsonProperty ("utcOffset")]
        public string UtcOffset { get; set; }

        [JsonProperty ("emails")]
        public List<EmailBundle> Emails { get; set; }

        // To satisfy Util.FromDDPMessageResult/Util.FromDDPMessageFields generic restriction
        public User ()
        {
        }

        public User (DDPMessageData ddpMessageData = null)
        {
            DDPMessageData = ddpMessageData;
        }

        public class EmailBundle
        {
            [JsonProperty ("address")]
            public string Address { get; set; }

            [JsonProperty ("verified")]
            public bool IsVerified { get; set; }
        }

    }
}

