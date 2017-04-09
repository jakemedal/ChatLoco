
namespace ChatLoco.Models.User_Model
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public UserSettingsModel Settings { get; set; }
    }
}