using System.ComponentModel.DataAnnotations.Schema;

namespace lab_dotnet_task.Models
{
    // TODO: Model uzytkownika w bazie
    public class MessageModel
    {
        public int Id { get; set; }
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public string Text { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
        public virtual UserModel FromUser { get; set; }
        public virtual UserModel ToUser { get; set; }
    }
}
