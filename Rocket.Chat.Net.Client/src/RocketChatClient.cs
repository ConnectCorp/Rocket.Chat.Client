using System;

using System.Diagnostics;
using WebSocket4Net;
using System.Collections.Generic;
using Net.DDP.Client;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rocket.Chat.Net.Client
{
    // TODO This class is NOT thread-safe yet
    public class RocketChatClient :  IDataSubscriber
    {

        // TODO Double check that thread safety is really gonna be needed.
        private readonly object connectLock = new object ();

        // The name of the collection of Rocket.Messages
        private const string MessageCollection = "stream-messages";

        private readonly DDPClient ddpClient;

        private string meteorUrl;

        private int loginRequestId;

        private bool isConnecting;

        private bool IsConnectedAndLoggedIn { get { return true; } }

        public string Session { get; set; }

        #region RocketChat subjects

        // Subjects should be avoided. However, this is one of the very few use cases where they are justifiable.
        // And this is also a consequence of the API that the DDPClient offers: they don't use either Events or async
        // functions, and instead rely on a plain and general message firehose (implemented as a callback:
        // IDataSubscriber).
        private AsyncSubject<bool> connectSubject;
        private AsyncSubject<LoginResponse> loginSubject;
        private IDictionary<string, Subject<Message>> messageStreamByRoom;

        #endregion

        public RocketChatClient (string serverUrl)
        {
            if (!IsValidWebsocketUrl (serverUrl)) {
//                throw new ArgumentException ("Invalid URL");
            }

            ddpClient = new DDPClient (this);
            loginSubject = new AsyncSubject<LoginResponse> ();
            messageStreamByRoom = new Dictionary<string, Subject<Message>> ();
            meteorUrl = serverUrl;
        }

        private IObservable<bool> Connect ()
        {
            lock (connectLock) {
                if (!isConnecting) {
                    try {
                        connectSubject = new AsyncSubject<bool> ();
                        isConnecting = true;
                        ddpClient.Connect (meteorUrl, false);
                    } catch (Exception) {
                        isConnecting = false;
                        throw;
                    }
                }
                return connectSubject.AsObservable ();
            }
        }

        public IObservable<Message> SubscribeToRoom (string roomName)
        {
            if (!messageStreamByRoom.ContainsKey (roomName)) {
                // TODO Decide on hot vs cold mode... will depend on the final API form.
                messageStreamByRoom.Add (roomName, new Subject<Message> ());
            }

            ddpClient.Subscribe (MessageCollection, new string[] { roomName });
            return messageStreamByRoom [roomName].AsObservable ();
        }

        public IObservable<LoginResponse> LoginWithUsernameAndPassword (string username, string password)
        {
            try {
                Connect ().Subscribe (
                    (IsOk => {
                        Debug.WriteLine ("OnNext: " + IsOk);
                        loginRequestId = ddpClient.Call ("login", new object[] { new Dictionary<string, object> () { {
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
                        });
                    }),
                    (exception => {
                        Debug.WriteLine ("OnError: " + exception);
                    })
                );
                return loginSubject.AsObservable ();
            } catch (Exception e) {
                throw LogAndReturnBack (e);
            }
        }

        public IObservable<LoginResponse> LoginWithJwtToken (string jwtToken)
        {
            try {
                Connect ().Subscribe (
                    (IsOk => {
                        Debug.WriteLine ("OnNext: " + IsOk);
                        loginRequestId = ddpClient.Call ("login", new object[] { new Dictionary<string, object> () { {
                                    "connect",
                                    jwtToken
                                }
                            }
                        });
                    }),
                    (exception => {
                        Debug.WriteLine ("OnError: " + exception);
                    })
                );
                return loginSubject.AsObservable ();
            } catch (Exception e) {
                throw LogAndReturnBack (e);
            }
        }

        public IObservable<Message> sendTextMessage (string message, string roomId)
        {
            IDictionary<string, string>[] args = new IDictionary<string, string>[1];
            args [0] = new Dictionary<string, string> ();
            args [0].Add ("rid", roomId);
            args [0].Add ("msg", message);

            ddpClient.Call ("sendMessage", args);
		
            return null;
        }

        public void DataReceived (dynamic data)
        {
            // This is the unsorted message firehose of messages received through the DDPClient.
            // Each message should be handled according to its type.
            try {
                IDictionary<string, object> dataObj = data;
                DDPType type = (DDPType)dataObj ["Type"];

                switch (type) {
                case DDPType.Connected:
                    notifyOfConnectionResult (true);
                    break;
                case DDPType.Failed:
                    notifyOfConnectionResult (false);
                    break;
                case DDPType.Added:
                case DDPType.Removed:
                case DDPType.Changed:
                    HandleDataMessage (data);
                    break;
                case DDPType.MethodResult:
                    HandleMethodResult (data);
                    break;
                case DDPType.Error:
                    HandleError (data);
                    break;
                }
            } catch (Exception ex) {
                Debug.WriteLine (ex);
            }
        }

        private void notifyOfConnectionResult (bool connectionWasOk)
        {
            lock (connectLock) {
                connectSubject.OnNext (connectionWasOk);
                connectSubject.OnCompleted ();
                connectSubject = null;
                isConnecting = false;
            }
        }

        private void HandleError (IDictionary<string, object> data)
        {
			
        }

        private void HandleMethodResult (IDictionary<string, object> data)
        {

            // TODO Un-HardCode string literals
            int requestId;
            int.TryParse ((string)data ["RequestingId"], out requestId);

            if (requestId == loginRequestId) {
                // This is the result of a login request
                var wasLoggedIn = !string.IsNullOrEmpty ((string)((IDictionary<string, object>)data.GetValue<object> ("Result"))?.GetValue<object> ("Token"));
                Error error = parseErrorObject (data);

                loginSubject.OnNext (new LoginResponse (wasLoggedIn, error));
                loginSubject.OnCompleted ();
            }
			
        }

        private void HandleDataMessage (IDictionary<string, object> data)
        {

            string collection = (string)data ["Collection"];

            switch (collection) {
            case MessageCollection:
                List<object> args = (List<object>)data.GetValue<object> ("Args", null);
                if (args != null) {
                    string roomId = (string)args [0];
                    IDictionary<string, object> uData = (IDictionary<string, object>)args [1];
                    string messageContent = (string)uData ["Msg"];
                    string messageSenderUsername = (string)uData ["Msg"];
                    if (messageStreamByRoom.ContainsKey (roomId)) {
                        var message = new Message (messageContent);
                        messageStreamByRoom [roomId].OnNext (message);
                    }
                }
                break;
            }
        }

        private bool IsValidWebsocketUrl (string url)
        {
            // TODO Do exhaustive testing of this, pretty sure it lets some incorrect urls in
            // (because of Uri.IsWellFormedUriString()).
            return !string.IsNullOrWhiteSpace (url)
            && (url.StartsWith ("ws://")
            || url.StartsWith ("wss://"))
            && Uri.IsWellFormedUriString (url, UriKind.Absolute);
        }

        private Exception LogAndReturnBack (Exception e)
        {
            Debug.WriteLine (e);
            return e;
        }

        /// <summary>
        /// Parses the error object.
        /// </summary>
        /// <returns>The error object, or null if not found</returns>
        /// <param name="data">Data.</param>
        private Error parseErrorObject (IDictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey ("Error")) {
                return null;
            }
            try {
                var errorDict = (IDictionary<string, object>)data ["Error"];
                string code = errorDict.GetValue<string> ("Error", null);
                string reason = errorDict.GetValue<string> ("Reason", null);
                string message = errorDict.GetValue<string> ("Message", null);
                string errorType = errorDict.GetValue<string> ("ErrorType", null);
                return new Error (code, reason, message, errorType);
            } catch (InvalidCastException e) {
                throw LogAndReturnBack (e);
            }
        }
    }
}

