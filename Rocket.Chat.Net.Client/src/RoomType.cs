using System;

namespace Rocket.Chat.Net.Client
{
    public class RoomType
    {
        internal string TypeCode { get; private set; }

        public static RoomType PrivateGroup { get; } = new RoomType ("p");

        public static RoomType Channel { get; } = new RoomType ("c");

        public static RoomType DirectMessage { get; } = new RoomType ("d");

        private RoomType (string typeCode)
        {
            TypeCode = typeCode;
        }
    }
}

