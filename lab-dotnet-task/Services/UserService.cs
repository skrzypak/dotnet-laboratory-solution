using lab_dotnet_task.Dtos;
using lab_dotnet_task.Interfaces;
using lab_dotnet_task.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace lab_dotnet_task.Services
{
    public class UserService : IUserService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHeaderContextService _headerContextService;
        private readonly IWebSocketService _webSocketService;

        public UserService(IServiceScopeFactory scopeFactory, IHeaderContextService headerContextService, IWebSocketService webSocketService)
        {
            _scopeFactory = scopeFactory;
            _headerContextService = headerContextService;
            _webSocketService = webSocketService;
        }

        public async Task<DataDto<UserDataDto>> GetUsers()
        {
            DataDto<UserDataDto> users = new DataDto<UserDataDto>();

            using (var scope = _scopeFactory.CreateScope())
            {
                // Otwarcie polaczenia do bazy danych
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                // TODO: wyslanie listy uzytkownikiow klientowi wraz z statusem aktywnosci na podstawie polaczen ws

                users.data = db.Users
                        .AsNoTracking()
                        .Select(x => new UserDataDto
                        {
                            user_id = x.Id,
                            user_username = x.Username,
                            online = _webSocketService.IsUserOnline(x.Id),
                        })
                        .ToList();
            }

            return users;
        }

        public async Task<object> Login(LoginDto dto)
        {
            UserModel? model;

            using (var scope = _scopeFactory.CreateScope())
            {
                // Otwarcie polaczenia do bazy danych
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                model = db.Users
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Username == dto.user_name && x.Password == dto.user_password);

            }

            // TODO: logowanie - zaszyfrowane ciasteczko logowania
            // TODO: Zawrzyj w nim potrzebne claim'y (ID, ...)

            if (model == null)
            {
                throw new BadHttpRequestException("Username or password incorrect");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, model.Id.ToString()),
                new Claim(ClaimTypes.Authentication, true.ToString()),
            };

            await _headerContextService.GetHttpContext()!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(15),
                    IssuedUtc = DateTimeOffset.UtcNow,
                });

            return new { 
                loggedin = true, 
                user_name = model.Username,
                user_id = model.Id
            };
        }

        public async Task<object> Logout()
        {
            await _headerContextService.GetHttpContext()!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new { loggedin = false };
        }

        public async Task<object> Register(RegisterDto dto)
        {

            if(dto.user_name.Length  == 0 || dto.user_password.Length == 0)
            {
                throw new BadHttpRequestException("Incorrect username or password");
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                // Otwarcie polaczenia do bazy danych
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                // Dodanie rekordu
                db.Users.Add(new UserModel
                {
                    Username = dto.user_name,
                    Password = dto.user_password,
                });

                // Zapisanie zmian
                await db.SaveChangesAsync();
            }

            // Zwracanie typu anonimowego
            return new { register = true };
        }

        public string TestGet()
        {
            return "testGet working";
        }
    }
}

