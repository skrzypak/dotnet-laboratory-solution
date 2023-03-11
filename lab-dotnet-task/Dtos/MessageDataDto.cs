using System;

namespace lab_dotnet_task.Dtos
{
    public class MessageDataDto
    {
        // TODO: Dodaj wartosci jakie beda udostepnione klientowi
        public Guid message_from_user_id { get; set; }
        public Guid message_to_user_id { get; set; }
        public string message_text { get; set; }
        public DateTimeOffset message_date { get; set; }
    }
}
