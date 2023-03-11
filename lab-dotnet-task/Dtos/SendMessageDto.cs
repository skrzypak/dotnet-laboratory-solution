namespace lab_dotnet_task.Dtos
{
    public class SendMessageDto
    {
        public Guid message_to_user_id { get; set; }
        public string message_text { get; set; } = "";
    }
}
