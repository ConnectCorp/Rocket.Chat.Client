using System;

namespace Rocket.Chat.Net.Client
{
    public class CanAccessRoomResponse : DDPBaseModel
    {
        // To satisfy Util.FromDDPMessageResult/Util.FromDDPMessageFields generic restriction
        public CanAccessRoomResponse ()
        {
        }
    }
}

