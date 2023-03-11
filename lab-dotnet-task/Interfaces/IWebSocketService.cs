using lab_dotnet_task.Dtos;
using System.Net.WebSockets;

namespace lab_dotnet_task.Interfaces
{
    public interface IWebSocketService
    {
        public Task Process(WebSocket webSocket, Guid userId);
        public bool IsUserOnline(Guid userId);
        public void SendMessageToUser(Guid recipientUserId, string message);
    }
}
