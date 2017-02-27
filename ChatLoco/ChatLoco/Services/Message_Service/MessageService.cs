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

        public static MessageDTO CreateMessage(int userId, int chatroomId, string rawMessage, string userHandle)
        {
            System.Diagnostics.Debug.WriteLine("creating message!!");
            ChatLocoContext DbContext = new ChatLocoContext();
            try
            {
                MessageDTO m = new MessageDTO()
                {
                    UserId = userId,
                    ChatroomId = chatroomId,
                    RawMessage = rawMessage,
                    DateCreated = DateTime.Now
            };

                string currentTime = m.DateCreated.ToString("MM/dd [h:mm:ss tt]");
                string formattedMessage = string.Format("{0} [{1}] : {2}", currentTime, userHandle, rawMessage);

                m.FormattedMessage = formattedMessage;

                //add it to the table and commit the changes
                System.Diagnostics.Debug.WriteLine("adding message!!");
                DbContext.Messages.Add(m);
                DbContext.SaveChanges();

                MessageCache.Add(m.Id, m);


                return m;
            }
            catch(Exception e)
            {
                return null;
            }
        }

    }
}