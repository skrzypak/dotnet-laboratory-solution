using lab_dotnet_task.Dtos;
using lab_dotnet_task.Interfaces;
using lab_dotnet_task.Models;
using Microsoft.EntityFrameworkCore;

namespace lab_dotnet_task.Services
{
    public class MessageService : IMessageService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHeaderContextService _headerContextService;
        private readonly IWebSocketService _webSocketService;

        public MessageService(IServiceScopeFactory scopeFactory, IHeaderContextService headerContextService, IWebSocketService webSocketService)
        {
            _scopeFactory = scopeFactory;
            _headerContextService = headerContextService;
            _webSocketService = webSocketService;
        }

        public async Task<DataDto<MessageDataDto>> GetMessagesBetweenUser(Guid userId)
        {

            DataDto<MessageDataDto> messages = new DataDto<MessageDataDto>();
            var currUserId = _headerContextService.GetUserId();

            using (var scope = _scopeFactory.CreateScope())
            {
                // Otwarcie polaczenia do bazy danych
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                // TODO: Pobranie rozmowy z uzytkownikiem o danym ID

                messages.data = db.Messages
                    .AsNoTracking()
                    .Where(x => x.FromUserId == currUserId || x.FromUserId == userId || x.ToUserId == currUserId || x.ToUserId == userId)
                    .Select(x => new MessageDataDto
                    {
                        message_from_user_id = x.FromUserId,
                        message_to_user_id = x.ToUserId,
                        message_text = x.Text,
                        message_date = x.Date,
                    })
                    .ToList();
            }

            return messages;
        }

        public async Task<object> SendMessageToUser(SendMessageDto dto)
        {
            // TODO: Sporzadz model i przeprowadz migracje bazy danych

            // Pobranie ID uzytkownika z kontekstu HTTP
            var messageFromUserId = _headerContextService.GetUserId();

            using (var scope = _scopeFactory.CreateScope())
            {
                // Otwarcie polaczenia do bazy danych
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                // TODO: Zapisanie wiadomosci do bazy danych
                // ...

                UserModel? messageToUserModel = db.Users
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == dto.message_to_user_id);

                if (messageToUserModel == null)
                {
                    return new
                    {
                        error = "Podany użytkownik nie istnieje"
                    };
                }

                db.Messages.Add(new MessageModel
                {
                     FromUserId = messageFromUserId,
                     ToUserId = messageToUserModel.Id,
                     Text = dto.message_text,
                     Date = DateTimeOffset.UtcNow
                });

                await db.SaveChangesAsync();
            }

            if (_webSocketService.IsUserOnline(dto.message_to_user_id))
            {
                // TODO: uzupelnij implementacje metody wyslania wiadomosci do odbiory w pliku WebSocketService.cs
                _webSocketService.SendMessageToUser(dto.message_to_user_id, dto.message_text);
            }

            if (messageFromUserId != dto.message_to_user_id)
            {
                if (_webSocketService.IsUserOnline(messageFromUserId))
                {
                    // TODO: uzupelnij implementacje metody wyslania wiadomosci do nadawcy jeżeli odbiorca nie jest nadawca w pliku WebSocketService.cs
                    _webSocketService.SendMessageToUser(messageFromUserId, dto.message_text);
                }
            }

            // Zwracanie typu anonimowego
            return new { 
                sending = true,
                message_text = dto.message_text,
                message_to_user_id = dto.message_to_user_id
            };
        }
    }
}

