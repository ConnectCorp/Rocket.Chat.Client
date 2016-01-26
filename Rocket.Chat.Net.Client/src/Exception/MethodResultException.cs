using System;

namespace Rocket.Chat.Net.Client
{
    public class MethodResultException : Exception
    {
        public MethodResultException (string message = null) : base (message)
        {
        }
    }
}

