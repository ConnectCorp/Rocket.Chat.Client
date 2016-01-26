using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rocket.Chat.Net.Client
{
    public class Messages : DDPBaseModel
    {
        [JsonProperty ("messages")]
        public List<Message> MessagesCollection { get; set; }

        public Messages ()
        {
        }
    }
}

