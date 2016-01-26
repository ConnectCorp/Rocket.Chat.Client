using System;
using Net.DDP.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Rocket.Chat.Net.Client
{
    public class MessageContainer : DDPBaseModel
    {
        [JsonProperty ("args")]
        private List<object> Args { get; set; }

        [JsonProperty ("userId")]
        public string UserID { get; set; }

        [JsonProperty ("type")]
        public string Type { get; set; }

        [JsonProperty ("subscriptionId")]
        public string SubscriptionId { get; set; }

        // This is unsafe but at his point we don't want to mask any change in the expected payload format
        [JsonIgnore]
        public string RoomId => (string) Args?[0];

        // This is unsafe but at his point we don't want to mask any change in the expected payload format
        [JsonIgnore]
        public Message MessagePayload => (Message) ((JContainer)Args?[1])?.ToObject<Message>();

        // To satisfy Util.FromDDPMessageResult/Util.FromDDPMessageFields generic restriction
        public MessageContainer ()
        {
        }
    }
}

