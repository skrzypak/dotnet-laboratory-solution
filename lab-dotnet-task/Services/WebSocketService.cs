using lab_dotnet_task.Interfaces;
using System.Net.WebSockets;
using System.Text;

namespace lab_dotnet_task.Services
{
    public class WebSocketService : IWebSocketService
    {
        // Przechowuje ID aktywnych klientow i ich sockety
        private readonly IDictionary<Guid, WebSocket> connections = new Dictionary<Guid, WebSocket>();

        public async Task Process(WebSocket webSocket, Guid userId)
        {
            lock (connections)
            {
                // Dodanie polaczenia klienta do kolekcji
                connections.Add(userId, webSocket);
            }

            await SendToAll("{\"status\":2}");

            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                // Oczekiwanie na odebranie nowej wiadomosci z polaczenia ws
                var segment = new ArraySegment<byte>(buffer);

                // Czekamy na wiadomosc z polaczenia
                var receiveResult = await webSocket.ReceiveAsync(segment, CancellationToken.None);

                // Sprawdzamy typ odbieranej wiadmosci - wylapujemy wylacznie tekstowa
                if (receiveResult.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.Default.GetString(segment).TrimEnd('\0');

                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        // Przeslanie otzymana wiadomosci na wszystkie dostepne polaczenia
                        await SendToAll(message);
                    }
                } else if(receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(
                        receiveResult.CloseStatus!.Value,
                        receiveResult.CloseStatusDescription,
                        CancellationToken.None);
                }
            }

            lock (connections)
            {
                // Usuniecie polaczenia klienta do kolekcji
                connections.Remove(userId);
            }
        }

        public async void SendMessageToUser(Guid recipientUserId, string message)
        {
            // TODO: napisz tresc funkcji

            WebSocket? recipientWs;

            lock (connections)
            {
                connections.TryGetValue(recipientUserId, out recipientWs);
            }
            

            if(recipientWs == null)
            {
                return;
            }

            var bytes = Encoding.Default.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(bytes);

            // Wysylamy wiadomosc do innego klienta
            await recipientWs.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public bool IsUserOnline(Guid userId)
        {
            return connections.ContainsKey(userId);
        }

        private async Task SendToAll(string message)
        {
            // TODO: napisz tresc funkcji

            IEnumerable<WebSocket> clientsConnections;

            lock (connections)
            {
                clientsConnections = connections.Values.ToList();
            }

            // Asynchroniczne wysylamy wiadomosci do aktwynych klientow
            var tasks = clientsConnections.Select(async ws =>
            {
                var bytes = Encoding.Default.GetBytes(message);
                var arraySegment = new ArraySegment<byte>(bytes);

                // Wysylamy wiadomosc do innego klienta
                await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            });

            await Task.WhenAll(tasks);
        }
    }
}
