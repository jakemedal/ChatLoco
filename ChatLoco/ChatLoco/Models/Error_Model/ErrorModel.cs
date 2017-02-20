
namespace ChatLoco.Models.Error_Model
{
    public class ErrorModel
    {
        public ErrorModel(string error)
        {
            ErrorMessage = error;
        }

        public string ErrorMessage { get; set; }
    }
}