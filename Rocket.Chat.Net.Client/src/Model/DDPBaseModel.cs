using System;
using Net.DDP.Client;
using Newtonsoft.Json;

namespace Rocket.Chat.Net.Client
{
    /// <summary>
    /// This is the base class for any DDP message resulting from a DDP method call to the Rocket.Chat server.
    /// Different data models for different method call responses will subclass this class.
    /// </summary>
    public class DDPBaseModel
    {
        /// <summary>
        /// Fallback container to hack around the fact that the server sends in different types of values for the result
        /// field so, when the result can't be parsed to any particular json object model, it is just assigned here.
        /// So this field is NULL whenever the result is a proper JSON object (and could therefore be parsed as a
        /// subclass of this class), and NOT NULL for other types of simple values such as strings and numbers.
        /// </summary>
        /// <value>A result of any type other than a full json object: strings, numbers, etc.</value>
        [JsonIgnore]
        public object Result { get; set; }

        /// <summary>
        /// Contains the DDP metadata associated with this DDP message.
        /// </summary>
        /// <value>The DDP message data.</value>
        public DDPMessageData DDPMessageData { get; set; }
    }
}

