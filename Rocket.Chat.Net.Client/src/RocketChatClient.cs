using System;

using System.Diagnostics;
using WebSocket4Net;
using System.Collections.Generic;
using Net.DDP.Client;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Collections;
using Newtonsoft.Json;

namespace Rocket.Chat.Net.Client
{
    public class RocketChatClient
    {
        // TODO Implement the required level of thread safety.
        private readonly object connectLock = new object ();
        private DDPClient ddpClient;
        private string meteorUrl;

        private bool IsConnectedAndLoggedIn { get { return true; } }

        public RocketChatClient (string serverUrl, DDPClient ddpClient)
        {
            this.ddpClient = ddpClient;
            meteorUrl = serverUrl;
        }

        private IObservable<bool> Connect ()
        {
            return ddpClient.Connect (meteorUrl).Select (m => DDPType.Connected.Equals (m.Type));
        }

        #region Rocket.Chat subscriptions

        public IObservable<MessageContainer> SubscribeToMessageStream ()
        {
            return ddpClient.Subscribe (RCSubscription.Messages)
                .Where (message => message.Fields != null)
                .Select (ddpMessage => Util.FromDDPMessageField<MessageContainer> (ddpMessage));
        }

        public IObservable<User> SubscribeToFilteredUserStream ()
        {
            return ddpClient.Subscribe (RCSubscription.FilteredUsers, new object [] { })
                .Where (message => message.Fields != null)
                .Select (ddpMessage => Util.FromDDPMessageField<User> (ddpMessage));
        }

        public IObservable<User> SubscribeToActiveUserStream ()
        {
            return ddpClient.Subscribe (RCSubscription.ActiveUsers)
                .Where (message => message.Fields != null)
                .Select (ddpMessage => Util.FromDDPMessageField<User> (ddpMessage));
        }

        public IObservable<User> SubscribeToUserData ()
        {
            return ddpClient.Subscribe (RCSubscription.UserData)
                .Where (message => message.Fields != null)
                .Select (ddpMessage => Util.FromDDPMessageField<User> (ddpMessage));
        }

        public IObservable<User> SubscribeToFullUserData (string filter, int limit)
        {
            return ddpClient.Subscribe (RCSubscription.FullUserData, new object[] { filter, limit })
                .Where (message => message.Fields != null)
                .Select (ddpMessage => Util.FromDDPMessageField<User> (ddpMessage));
        }

        public IObservable<RoomNotification> SubscribeToRoomNotifications ()
        {
            return ddpClient.Subscribe (RCSubscription.NotifyRoom)
                .Where (message => message.Fields != null)
                .Select (ddpMessage => Util.FromDDPMessageField<RoomNotification> (ddpMessage));
        }

        public IObservable<UserNotification> SubscribeToUserNotifications ()
        {
            return ddpClient.Subscribe (RCSubscription.NotifyUser)
                .Where (message => message.Fields != null)
                .Select (ddpMessage => Util.FromDDPMessageField<UserNotification> (ddpMessage));
        }

        public IObservable<AllNotification> SubscribeToAllNotifications ()
        {
            return ddpClient.Subscribe (RCSubscription.NotifyAll)
                .Where (message => message.Fields != null)
                .Select (ddpMessage => Util.FromDDPMessageField<AllNotification> (ddpMessage));
        }

        public IObservable<Room> SubscribeToRooms ()
        {
            return ddpClient.Subscribe (RCSubscription.Rooms)
                .Where (message => message.Fields != null)
                .Select (ddpMessage => Util.FromDDPMessageField<Room> (ddpMessage));
        }

        public IObservable<T> SubscribeToCollection<T> (RCCollection collection) where T:DDPBaseModel, new()
        {
            return ddpClient.GetCollectionStream (collection.Name)
                .Select (ddpMessage => Util.FromDDPMessageField<T> (ddpMessage));
        }

        #endregion

        #region Rocket.Chat methods

        public IObservable<LoginResponse> LoginWithUsernameAndPassword (string username, string password)
        {
            try {
                var requestArgs = buildLoginWithUsernameAndPasswordRequest (username, password);
                return Connect ()
                    .Where (ok => ok)
                    .SelectMany (ok => ddpClient.Call ("login", requestArgs)
                        .Select (ddpMessage => Util.FromDDPMessageResult<LoginResponse> (ddpMessage)));
            } catch (Exception e) {
                throw LogAndReturnBack (e);
            }
        }

        private object[] buildLoginWithUsernameAndPasswordRequest (string username, string password)
        {
            return new object[] { new Dictionary<string, object> () { {
                        "user",
                        new Dictionary<string, string> () { {
                                "username",
                                username
                            }
                        }
                    }, {
                        "password",
                        password
                    }
                }
            };
        }

        public IObservable<LoginResponse> LoginWithJwtToken (string jwtToken)
        {
            try {
                var requestArgs = buildLoginWithJWTTokenRequest (jwtToken);
                return Connect ()
                    .Where (ok => ok)
                    .SelectMany (ok => ddpClient.Call ("login", requestArgs)
                        .Where (message => message.Result != null)
                        .Select (ddpMessage => Util.FromDDPMessageResult<LoginResponse> (ddpMessage)));
            } catch (Exception e) {
                throw LogAndReturnBack (e);
            }
        }

        private object[] buildLoginWithJWTTokenRequest (string jwtToken)
        {
            return new object[] { new Dictionary<string, object> () { {
                        "connect",
                        jwtToken
                    }
                }
            };
        }

        public IObservable<Message> SendTextMessage (string message, string roomId)
        {
            if (string.IsNullOrWhiteSpace (roomId))
                throw new ArgumentNullException ("roomId");

            if (string.IsNullOrWhiteSpace (message))
                throw new ArgumentNullException ("message");

            IDictionary<string, string>[] args = new IDictionary<string, string>[1];
            args [0] = new Dictionary<string, string> ();
            args [0].Add ("rid", roomId);
            args [0].Add ("msg", message);
        
            return ddpClient.Call (RCMethod.SendMessage, args)
                .Select (ddpMessage => Util.BuildMessageFromResponse<Message> (ddpMessage));
        }

        public IObservable<CreateDirectMessageResponse> CreateDirectMessageRoom (string username)
        {
            return ddpClient.Call (RCMethod.CreateDirectMessage, new string[] { username })
                .Select (ddpMessage => Util.FromDDPMessageResult<CreateDirectMessageResponse> (ddpMessage));
        }

        public IObservable<EraseRoomResponse> EraseRoom (string roomId)
        {
            return ddpClient.Call (RCMethod.EraseRoom, new string[] { roomId })
                .Select (ddpMessage => Util.FromDDPMessageResult<EraseRoomResponse> (ddpMessage));
        }

        public IObservable<ArchiveRoomResponse> ArchiveRoom (string roomId)
        {
            return ddpClient.Call (RCMethod.ArchiveRoom, new string[] { roomId })
                .Select (ddpMessage => Util.FromDDPMessageResult<ArchiveRoomResponse> (ddpMessage));
        }

        public IObservable<UnarchiveRoomResponse> UnarchiveRoom (string roomId)
        {
            return ddpClient.Call (RCMethod.UnarchiveRoom, new string[] { roomId })
                .Select (ddpMessage => Util.FromDDPMessageResult<UnarchiveRoomResponse> (ddpMessage));
        }

        public IObservable<LeaveRoomResponse> LeaveRoom (string roomId)
        {
            return ddpClient.Call (RCMethod.LeaveRoom, new string[] { roomId })
                .Select (ddpMessage => Util.FromDDPMessageResult<LeaveRoomResponse> (ddpMessage));
        }

        public IObservable<JoinRoomResponse> JoinRoom (string roomId)
        {
            return ddpClient.Call (RCMethod.JoinRoom, new string[] { roomId })
                .Select (ddpMessage => Util.FromDDPMessageResult<JoinRoomResponse> (ddpMessage));
        }

        public IObservable<CanAccessRoomResponse> CanAccessRoom (string roomId, string userId)
        {
            return ddpClient.Call (RCMethod.CanAccessRoom, new string[] { roomId, userId })
                .Select (ddpMessage => Util.FromDDPMessageResult<CanAccessRoomResponse> (ddpMessage));
        }

        public IObservable<ChannelsListResponse> ChannelList ()
        {
            return ddpClient.Call (RCMethod.ChannelsList)
                .Select (ddpMessage => Util.FromDDPMessageResult<ChannelsListResponse> (ddpMessage));
        }

        public IObservable<AddUserToRoomResponse> AddUserToRoom (string roomId, string username)
        {
            IDictionary<string, string>[] args = new IDictionary<string, string>[1];
            args [0] = new Dictionary<string, string> ();
            args [0].Add ("rid", roomId);
            args [0].Add ("username", username);

            return ddpClient.Call (RCMethod.AddUserToRoom, args)
                .Select (ddpMessage => Util.BuildMessageFromResponse<AddUserToRoomResponse> (ddpMessage));
        }

        public IObservable<CreateChannelResponse> CreateChannel (string name)
        {
            return ddpClient.Call (RCMethod.CreateChannel, new object[] { name, new short [] { } })
                .Select (ddpMessage => Util.FromDDPMessageResult<CreateChannelResponse> (ddpMessage));
        }

        public IObservable<CreatePrivateGroupResponse> CreatePrivateGroup (string name)
        {
            return ddpClient.Call (RCMethod.CreatePrivateGroup, new object[] { name, new short [] { } })
                .Select (ddpMessage => Util.FromDDPMessageResult<CreatePrivateGroupResponse> (ddpMessage));
        }

        public IObservable<GetGroupByNameOrIdResponse> GetGroupByNameOrId (string nameOrId)
        {
            return ddpClient.Call (RCMethod.GetRoomIdByNameOrId, new object[] { nameOrId })
                .Select (ddpMessage => Util.BuildMessageFromResponse<GetGroupByNameOrIdResponse> (ddpMessage));
        }

        public IObservable<GetTotalChannelsResponse> GetTotalChannels ()
        {
            return ddpClient.Call (RCMethod.GetTotalChannels)
                .Select (ddpMessage => Util.BuildMessageFromResponse<GetTotalChannelsResponse> (ddpMessage));
        }

        public IObservable<Messages> LoadHistory (string roomId, long? end, int limit, long ls)
        {
            return ddpClient.Call (RCMethod.LoadHistory, new object[] { roomId, end, limit, ls })
                .Select (ddpMessage => Util.BuildMessageFromResponse<Messages> (ddpMessage));
        }

        #endregion

        private Exception LogAndReturnBack (Exception e)
        {
            Debug.WriteLine (e);
            return e;
        }

        public static RocketChatClientBuilder WithUrl (string url)
        {
            var builder = new RocketChatClientBuilder () { serverUrl = url };

            return builder;
        }

        public class RocketChatClientBuilder
        {
            internal string serverUrl;
            private DDPClient ddpClient;

            public RocketChatClient Build ()
            {
                ValidateFields ();
                AssignDefaults ();
                
                return new RocketChatClient (serverUrl, ddpClient);
            }

            private void AssignDefaults ()
            {
                if (ddpClient == null)
                    ddpClient = new DDPClient ();
            }

            private void ValidateFields ()
            {
                // TODO Strengthen this
                if (string.IsNullOrWhiteSpace (serverUrl))
                    throw new ArgumentNullException ("serverUrl");
            }
        }
    }
}

