using lab_dotnet_task.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace lab_dotnet_task.Interfaces
{
    public interface IMessageService
    {
        public Task<object> SendMessageToUser(SendMessageDto dto);
        public Task<DataDto<MessageDataDto>> GetMessagesBetweenUser(Guid userId);
    }
}
