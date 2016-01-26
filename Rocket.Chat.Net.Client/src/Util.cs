using System;
using Net.DDP.Client;
using Newtonsoft.Json;

namespace Rocket.Chat.Net.Client
{
    public static class Util
    {
        public const string True = "true";
        public const string False = "false";

        public static T FromDDPMessageResult<T> (DDPMessage ddpMessage) where T : DDPBaseModel, new()
        {
            if (ddpMessage == null)
                throw new ArgumentNullException ("ddpMessage");

            T response = null;
            if (ddpMessage.Result != null) {
                response = JsonConvert.DeserializeObject<T> (ddpMessage.Result);
            } else {
                response = new T ();
            }
            response.DDPMessageData = ddpMessage.DDPMessageData;
            return response;
        }

        public static T FromDDPMessageField<T> (DDPMessage ddpMessage) where T : DDPBaseModel, new()
        {
            if (ddpMessage == null)
                throw new ArgumentNullException ("ddpMessage");

            T response = null;
            if (ddpMessage.Fields != null) {
                response = JsonConvert.DeserializeObject<T> (ddpMessage.Fields);
            } else {
                response = new T ();
            }
            response.DDPMessageData = ddpMessage.DDPMessageData;
            return response;
        }

        public static T BuildMessageFromResponse<T> (DDPMessage ddpMessage) where T : DDPBaseModel, new()
        {
            try {
                return Util.FromDDPMessageResult<T> (ddpMessage);
            } catch (Exception e) {
                // This is the sad consequence of the server using different data types for the field Result.
                // Sometimes that field can be a boolean, others a string, and others a json object (in which case it 
                // is handled in the try block). Here we just assign the value to a generic object type and let the 
                // client handle it appropriately.
                if (True.Equals (ddpMessage.Result)) {
                    return new T () { Result = true, DDPMessageData = ddpMessage.DDPMessageData };
                } else if (False.Equals (ddpMessage.Result)) {
                    return new T () { Result = false };
                } else {
                    return new T () { Result = ddpMessage.Result, DDPMessageData = ddpMessage.DDPMessageData };
                }
            }
        }
    }
}

