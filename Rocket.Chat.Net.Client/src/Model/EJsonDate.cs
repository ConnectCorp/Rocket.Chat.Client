using System;
using Newtonsoft.Json;

namespace Rocket.Chat.Net.Client
{
    public class EJsonDate
    {
        [JsonProperty ("$date")]
        public long Date { get; set; }
    }
}

