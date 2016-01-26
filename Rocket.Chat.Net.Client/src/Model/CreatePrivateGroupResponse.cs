using System;
using Newtonsoft.Json;

namespace Rocket.Chat.Net.Client
{
    public class CreatePrivateGroupResponse : DDPBaseModel
    {
        [JsonProperty ("rid")]
        public string RoomId { get; set; }

        // To satisfy Util.FromDDPMessageResult/Util.FromDDPMessageFields generic restriction
        public CreatePrivateGroupResponse ()
        {
        }
    }
}

