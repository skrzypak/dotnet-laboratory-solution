using lab_dotnet_task.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace lab_dotnet_task.Controllers;

public class WebSocketController : ControllerBase
{

    private readonly IWebSocketService _service;

    public WebSocketController(IWebSocketService service)
    {
        _service = service;
    }

    // ##########################################
    //
    // CZESC II
    //
    // ##########################################

    // TODO: zabezpiecz przed nieuwierzytelnionymi uzytkownikami 
    [HttpGet("/ws")]
    [Authorize]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            // Pobranie ws nowo polaczonego klienta
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            // Pobranie ID usera z ciasteczka
            var claim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid userId = new Guid(claim);

            // Przeslanie ws do serwisu obslugujacego polaczenia
            await _service.Process(webSocket, userId);
        }
        else
        {
            // Jezeli zapytanie nie bylo ws
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

}
