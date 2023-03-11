using lab_dotnet_task.Dtos;
using lab_dotnet_task.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab_dotnet_task.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        public ApiController(IUserService userService, IMessageService messageService)
        {
            _userService = userService;
            _messageService = messageService;
        }

        // ##########################################
        //
        // CZESC I
        //
        // ##########################################

        [HttpGet("/api/test-get")]
        public ActionResult<string> TestGet()
        {
            string str = _userService.TestGet();

            // Konwertuje automatycznie na JSON
            return Ok(str);
        }

        // rejestracja uzytkownika
        [AllowAnonymous]
        [HttpPost("/api/register")]
        public async Task<ActionResult<object>> Register([FromBody] RegisterDto dto)
        {

            try
            {
                var data = await _userService.Register(dto);

                // Konwertuje automatycznie na JSON
                return Ok(data);
            }
            catch (Exception)
            {
                // Zwracanie anonimowego typu
                // Konwertuje automatycznie na JSON
                return Ok(new { register = false });
            }
        }

        // logowanie uzytkownika
        [AllowAnonymous]
        [HttpPost("/api/login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
        {
            // TODO: stworz potrzebne obiekty DTO i dodaj je do definicji metod

            try
            {
                var data = await _userService.Login(dto);

                // Konwertuje automatycznie na JSON
                return Ok(data);
            }
            catch (Exception)
            {
                // Zwracanie anonimowego typu
                // Konwertuje automatycznie na JSON
                return Ok(new { loggedin = false });
            }
        }

        // test-sprawdzenie czy klient jest zalogowany
        [Authorize]
        [HttpGet("/api/login-test")]
        public ActionResult<object> LoginTest()
        {
            // Konwertuje automatycznie na JSON
            return Ok(new { loggedin = true });
        }

        // wylogowowanie uzytkownika
        [HttpGet("/api/logout")]
        public async Task<ActionResult<object>> Logout()
        {
            var data = await _userService.Logout();

            // Konwertuje automatycznie na JSON
            return Ok(data);
        }

        // lista wszystkich uzytkownikow
        [HttpGet("/api/users")]
        [Authorize]
        public async Task<ActionResult<DataDto<UserDataDto>>> GetUsers()
        {
            DataDto<UserDataDto> data = await _userService.GetUsers();

            // Konwertuje automatycznie na JSON
            return Ok(data);
        }

        // ##########################################
        //
        // CZESC III
        //
        // ##########################################

        // TODO: stwórz endpoint, ktory bedzie odpowiedzialny za wysylanie wiadomosci do uzytkownika o podanym ID
        [HttpPost("/api/messages")]
        [Authorize]
        public async Task<ActionResult<DataDto<MessageDataDto>>> SendMessageToUser([FromBody] SendMessageDto dto)
        {
            try
            {
                object data = await _messageService.SendMessageToUser(dto);

                // Konwertuje automatycznie na JSON
                return Ok(data);
            } catch(Exception ex)
            {
                return Ok(new
                {
                    error = ex.Message
                });
            }
        }

        // pobranie rozmowy pomiedzy danym uzytkownikiem
        [HttpGet("/api/messages")]
        [Authorize]
        public async Task<ActionResult<DataDto<MessageDataDto>>> GetMessagesBetweenUser([FromQuery] Guid userId)
        {
            DataDto<MessageDataDto> data = await _messageService.GetMessagesBetweenUser(userId);

            // Konwertuje automatycznie na JSON
            return Ok(data);
        }
    }
}
