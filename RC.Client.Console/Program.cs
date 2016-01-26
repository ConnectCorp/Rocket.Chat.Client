using System;
using Rocket.Chat.Net.Client;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reactive.Linq;
using Net.DDP.Client;

namespace RC.Client.Console
{
    class MainClass
    {
        private const string JWT_TEST_SIMON_DOT_CONNECT = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImsxIiwidHlwIjoiSldUIn0.eyJleH" +
                                                          "AiOiIxNDgyNzgzOTgyIiwic3ViIjoiMSIsInYiOjEsIm5hbWUiOiJzaW1vbi5jb25uZWN0IiwiZW1haWwiOiJzaW1vbkBjb25uZWN0Lm" +
                                                          "NvbSJ9.xMdbiC2v6-BucNW7H1MJRxD1uku5c9ctiMc7Rgvkfl-rdzShCfx_KuvdBOe_jgkA__-rmnufHuzPJoNfaXSpviq0alCwC4rLS" +
                                                          "wMdBPbKWeBa_FsppPjhmXRD8YOYbwLksuHZy8ZQa2KR6UHbsj5Jg-KXwadoMq5CAklL22W5s-I";
        private RocketChatClient chatClient;
        private bool isLoggedIn;


        public MainClass ()
        {
            System.Console.WriteLine ("Starting Rocket.Chat .NET Client...");

            isLoggedIn = false;

            chatClient = RocketChatClient.WithUrl ("ws://192.168.99.100:3000/websocket").Build ();
        }

        public void Play ()
        {
//            chatClient.LoginWithUsernameAndPassword ("john.doe", "simon").Subscribe (
//                response => {
//                    System.Console.WriteLine ("Login ok: " + response);
//                    isLoggedIn = true;
//                    chatClient.SubscribeToRoom ("GENERAL").Subscribe (
//                        message => {
//                            System.Console.WriteLine ("Simon.Connect: " + message.MessageContent);
//                        }
//                    );
//                },
//                ex => System.Console.WriteLine (ex)
//            );

            chatClient.LoginWithJwtToken (JWT_TEST_SIMON_DOT_CONNECT).Subscribe (
                response => {
                    System.Console.WriteLine ("Login ok");
                    isLoggedIn = response.HasToken;
                    chatClient.SubscribeToMessageStream ().Subscribe (
                        message => {
                            Debug.WriteLine ("Message: " + message);
                        }
                    );

                    chatClient.SubscribeToActiveUserStream ().Subscribe (
                        message => Debug.WriteLine (message)
                    );

                    chatClient.SubscribeToFilteredUserStream ().Subscribe (
                        message => Debug.WriteLine (message)
                    );

                    chatClient.SubscribeToUserData ().Subscribe (
                        message => Debug.WriteLine (message)
                    );

                    chatClient.SubscribeToFullUserData (null, 10).Subscribe (
                        message => Debug.WriteLine (message)
                    );

                    chatClient.SubscribeToCollection <User> (RCCollection.FilteredUsers).Subscribe (
                        message => Debug.WriteLine ("Message: " + message)
                    );

                    chatClient.SubscribeToRoomNotifications ().Subscribe (
                        message => Debug.WriteLine ("Message: " + message)
                    );

                    chatClient.SubscribeToUserNotifications ().Subscribe (
                        message => Debug.WriteLine ("Message: " + message)
                    );

                    chatClient.SubscribeToAllNotifications ().Subscribe (
                        message => Debug.WriteLine ("Message: " + message)
                    );

                    chatClient.SubscribeToRooms ().Subscribe (
                        message => Debug.WriteLine ("Message: " + message)
                    );

                    chatClient.SubscribeToCollection<Room> (RCCollection.Rooms
                    ).Subscribe (
                        message => Debug.WriteLine ("Message: " + message)
                    );

//                    chatClient.CreateDirectMessageRoom ("john.doe")
//                        .SelectMany (message => chatClient.SendTextMessage ("hi again dude", message.RoomId))
//                        .SelectMany (message => chatClient.LeaveRoom ("1fjkzKWDQPTiZfBcCX"))
//                        .Subscribe (m => Debug.WriteLine (m));

//                    chatClient.ChannelList ().Where (r => r.Channels != null).SelectMany (
//                        message => 
//                        chatClient.LeaveRoom (message.Channels.Find (channel => "test".Equals (channel.Name))?.Id)
//                    ).Subscribe (
//                        m => Debug.WriteLine (m)
//                    );

//                    chatClient.ChannelList ().Where (r => r.Channels != null).SelectMany (
//                        message => chatClient.ArchiveRoom (message.Channels.Find (channel => "awesome-group4".Equals (channel.Name))?.Id)
//                    ).Subscribe (
//                        m => Debug.WriteLine (m)
//                    );


//                    chatClient.CreatePrivateGroup ("awesome-group4").Where (m => m.RoomId != null)
//                                            .SelectMany (
//                        message => 
//                                chatClient.SendTextMessage ("hi awesome group", message.RoomId)
//                    )
////                                            .SelectMany (message => chatClient.LeaveRoom ("1fjkzKWDQPTiZfBcCX"))
//                                            .Subscribe (
//                        m => Debug.WriteLine (m)
//                    );

                   

//                    chatClient.ChannelList ().Where (r => r.Channels != null).SelectMany (
//                        message => 
//                        chatClient.AddUserToRoom (message.Channels.Find (channel => "awesomeness".Equals (channel.Name))?.Id, "simon.rojas")
//                    ).Subscribe (
//                        m => Debug.WriteLine (m.Result?.GetType())
//                    );

//                    chatClient.GetTotalChannels ().Subscribe (
//                        m => Debug.WriteLine (m)
//                    );

//                    chatClient.ChannelList ().Where (r => r.Channels != null).SelectMany (
//                        list => chatClient.GetGroupByNameOrId (list.Channels [0].Id)
//                    ).Subscribe (
//                        m => Debug.WriteLine (m.Result?.GetType ())
//                    );

//                    chatClient.ChannelList ().Where (r => r.Channels != null)
//                    .SelectMany (
//                            list => chatClient.LoadHistory (list.Channels [0].Id, null, 1000, 1453839928000)
//                    )
//                        .Subscribe (
//                        m => Debug.WriteLine (m.Result?.GetType ())
//                    );



                },
                ex => System.Console.WriteLine (ex)
            );

            startUILoop ();
        }

        public static void Main (string[] args)
        {
            MainClass mainClass = new MainClass ();
            mainClass.Play ();
        }

        public void startUILoop ()
        {
            Task<string> inputTask = System.Console.In.ReadLineAsync ();
            while (true) {
                if (inputTask.IsCompleted) {
                    string input = inputTask.Result;

                    if ("bye".Equals (input)) {
                        break;
                    }

                    if (isLoggedIn && !string.IsNullOrWhiteSpace (input)) {
//                        chatClient.SendTextMessage (input, "GENERAL");
                    }

                    inputTask = System.Console.In.ReadLineAsync ();
                }
                // Throttle the thread a little bit

                System.Threading.Thread.Sleep (10);
            }
            System.Console.WriteLine ("See you later!");
        }
    }
}
