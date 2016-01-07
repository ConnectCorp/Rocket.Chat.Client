using System;

namespace Rocket.Chat.Net.Client
{
	public class Message
	{
		public string MessageContent { get; private set; }

		public Message (string messageContent = null)
		{
			MessageContent = messageContent;
		}
	}
}

