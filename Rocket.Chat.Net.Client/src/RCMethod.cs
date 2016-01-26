using System;

namespace Rocket.Chat.Net.Client
{
    public static class RCMethod
    {
        public const string SendMessage = "sendMessage";
        public const string CreateDirectMessage = "createDirectMessage";
        public const string CreateChannel = "createChannel";
        public const string CreatePrivateGroup = "createPrivateGroup";
        public const string JoinRoom = "joinRoom";
        public const string LeaveRoom = "leaveRoom";
        public const string EraseRoom = "eraseRoom";
        public const string LoadHistory = "loadHistory";
        public const string LoadMissedMessages = "loadMissedMessages";
        public const string ChannelsList = "channelsList";
        public const string HideRoom = "hideRoom";
        public const string CanAccessRoom = "canAccessRoom";
        public const string AddUserToRoom = "addUserToRoom";
        public const string GetRoomIdByNameOrId = "getRoomIdByNameOrId";
        public const string GetTotalChannels = "getTotalChannels";
        public const string ArchiveRoom = "archiveRoom";
        public const string UnarchiveRoom = "unarchiveRoom";
    }
}

