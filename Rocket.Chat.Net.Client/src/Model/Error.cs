using System;

namespace Rocket.Chat.Net.Client
{
	public class Error
	{

		public string Code { get; }

		public string Reason { get; }

		public string Message { get; }

		public string ErrorType { get; }

		public Error (string code = null, string reason = null, string message = null, string errorType = null)
		{
			Code = code;
			Reason = reason;
			Message = message;
			ErrorType = errorType;
		}
	}
}

