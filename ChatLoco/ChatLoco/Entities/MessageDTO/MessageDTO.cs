using System;

namespace ChatLoco.Entities.MessageDTO
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChatroomId { get; set; }
        public string RawMessage { get; set; }
        public string FormattedMessage { get; set; }
        public DateTime DateCreated { get; set; }
        public int IntendedForUserId { get; set; }
        public string Style { get; set; }
    }
}