using System;
using Rocket.Chat.Net.Client;
using System.IO;
using System.Threading.Tasks;

namespace RC.Client.Console
{
    class MainClass
    {
        private RocketChatClient chatClient;
        private bool isLoggedIn;

        public MainClass ()
        {
            System.Console.WriteLine ("Starting Rocket.Chat .NET Client...");

            isLoggedIn = false;

            chatClient = new RocketChatClient ("localhost:3000");
        }

        public void Play ()
        {
            chatClient.LoginWithUsernameAndPassword ("Chatty.Client", "simon").Subscribe (
                response => {
                    System.Console.WriteLine ("Login ok");
                    isLoggedIn = true;
                    chatClient.SubscribeToRoom ("GENERAL").Subscribe (
                        message => {
                            System.Console.WriteLine ("Simon.Connect: " + message.MessageContent);
                        }
                    );
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
                        chatClient.sendTextMessage (input, "GENERAL");
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
