using System;

namespace Rocket.Chat.Net.Client
{
	public class LoginResponse : BaseResponse
	{
		public bool WasLoggedIn { get; private set; }

		public LoginResponse (bool wasLoggedIn, Error error = null)
		{
			WasLoggedIn = wasLoggedIn;
			Error = error;
		}
	}
}

