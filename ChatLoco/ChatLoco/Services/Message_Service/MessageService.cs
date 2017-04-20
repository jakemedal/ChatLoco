using ChatLoco.Entities.MessageDTO;
using ChatLoco.Services.User_Service;
using System;
using System.Collections.Generic;
using ChatLoco.DAL;
using System.Linq;


namespace ChatLoco.Services.Message_Service
{
    public static class MessageService
    {

        private static Dictionary<int, MessageDTO> MessageCache = new Dictionary<int, MessageDTO>();

        private static int uniqueId = 0;

        public static MessageDTO GetMessage(int messageId)
        {
            System.Diagnostics.Debug.WriteLine("getting meesages");
            ChatLocoContext DbContext = new ChatLocoContext();
            try
            {
                return MessageCache[messageId];//if it exists in the cache return it
            }
            catch(Exception e)//if it doesnt exist grab it from the database
            {
                MessageDTO message = DbContext.Messages.FirstOrDefault(msg => msg.Id == messageId);
                if (message != null)
                {
                    MessageCache.Add(message.Id, message);
                }
                return message;
            }
        }

        public static List<MessageDTO> GetRecentChatroomMessages(int chatroomId, int amount)
        {
            var messages = new List<MessageDTO>();
            ChatLocoContext DbContext = new ChatLocoContext();

            var results = DbContext.Messages.Where(m => m.ChatroomId == chatroomId && m.DateCreated != null).OrderBy(m => m.DateCreated).Take(amount);

            messages.AddRange(results);

            return messages;
        }

        //if chatroomId is -1, means a private chatroom
        public static List<MessageDTO> CreateMessages(int userId, int chatroomId, string rawMessage, string userHandle, int desiredUserId, string desiredUserHandle)
        {
            System.Diagnostics.Debug.WriteLine("creating message!!");
            ChatLocoContext DbContext = new ChatLocoContext();
            try
            {
                List<MessageDTO> createdMessages = new List<MessageDTO>();

                if (desiredUserId == -1)
                {
                    MessageDTO m = new MessageDTO()
                    {
                        UserId = userId,
                        ChatroomId = chatroomId,
                        RawMessage = rawMessage,
                        DateCreated = DateTime.Now,
                        IntendedForUserId = desiredUserId
                    };
                    string currentTime = m.DateCreated.ToString("MM/dd [h:mm:ss tt]");

                    string formattedMessage = string.Format("{0} [{1}] : {2}", currentTime, userHandle, rawMessage);
                    m.FormattedMessage = formattedMessage;
                    
                    DbContext.Messages.Add(m);
                    DbContext.SaveChanges();

                    MessageCache.Add(m.Id, m);

                    createdMessages.Add(m);
                }
                else
                {
                    //Create the private whisper
                    MessageDTO m = new MessageDTO()
                    {
                        UserId = userId,
                        ChatroomId = chatroomId,
                        RawMessage = rawMessage,
                        DateCreated = DateTime.Now,
                        IntendedForUserId = desiredUserId
                    };
                    string currentTime = m.DateCreated.ToString("MM/dd [h:mm:ss tt]");

                    string formattedMessage = string.Format("{0} [{1}] : {2}", currentTime, "From "+userHandle, rawMessage);
                    m.FormattedMessage = formattedMessage;

                    DbContext.Messages.Add(m);
                    DbContext.SaveChanges();

                    MessageCache.Add(m.Id, m);

                    createdMessages.Add(m);

                    //Create the receipt
                    MessageDTO m2 = new MessageDTO()
                    {
                        UserId = userId,
                        ChatroomId = chatroomId,
                        RawMessage = rawMessage,
                        DateCreated = DateTime.Now,
                        IntendedForUserId = userId
                    };

                    string formattedMessage2 = string.Format("{0} [{1}] : {2}", currentTime, "To " + desiredUserHandle, rawMessage);
                    m2.FormattedMessage = formattedMessage2;

                    DbContext.Messages.Add(m2);
                    DbContext.SaveChanges();

                    MessageCache.Add(m2.Id, m2);

                    createdMessages.Add(m2);
                }

                return createdMessages;
            }
            catch(Exception e)
            {
                return null;
            }
        }

    }
}