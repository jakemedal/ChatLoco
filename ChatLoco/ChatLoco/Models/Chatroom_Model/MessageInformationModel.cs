
namespace ChatLoco.Models.Chatroom_Service
{
    public class MessageInformationModel
    {
        public int Id { get; set; }
        public string FormattedMessage { get; set; }
        public int IntendedForUserId { get; set; }
        public string MessageStyle { get; set; }
    }
}