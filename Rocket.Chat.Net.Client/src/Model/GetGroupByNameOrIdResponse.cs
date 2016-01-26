using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rocket.Chat.Net.Client
{
    public class GetGroupByNameOrIdResponse : DDPBaseModel
    {
        [JsonProperty ("channels")]
        public List<Channel> Channels { get; set; }

        // To satisfy Util.FromDDPMessageResult/Util.FromDDPMessageFields generic restriction
        public GetGroupByNameOrIdResponse ()
        {
        }
    }
}

