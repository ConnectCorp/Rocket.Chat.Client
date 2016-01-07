using System;

namespace Rocket.Chat.Net.Client
{
	public abstract class BaseResponse
	{
		public Error Error { get; protected set; }
	}
}

