using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rocket.Chat.Net.Client
{
    public class ChannelsListResponse : DDPBaseModel
    {
        [JsonProperty ("channels")]
        public List<Channel> Channels { get; set; }

        // To satisfy Util.FromDDPMessageResult/Util.FromDDPMessageFields generic restriction
        public ChannelsListResponse ()
        {
        }
    }
}

