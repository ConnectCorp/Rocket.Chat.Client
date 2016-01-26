using System;

namespace Rocket.Chat.Net.Client
{
    public class RCCollection
    {

        public string Name { get; private set; }

        public static readonly RCCollection Rooms = new RCCollection ("rocketchat_subscription");
        public static readonly RCCollection FilteredUsers = new RCCollection ("filtered-users");
        public static readonly RCCollection StreamNotifyRoom = new RCCollection ("stream-notify-room");
        public static readonly RCCollection StreamNotifyUser = new RCCollection ("stream-notify-user");
        public static readonly RCCollection StreamNotifyAll = new RCCollection ("stream-notify-all");
        public static readonly RCCollection Users = new RCCollection ("users");

        private RCCollection (string name)
        {
            Name = name;
        }
    }
}

