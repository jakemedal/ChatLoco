using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Entities.MessageDTO
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChatroomId { get; set; }
        public string RawMessage { get; set; }
        public string FormattedMessage { get; set; }
    }
}