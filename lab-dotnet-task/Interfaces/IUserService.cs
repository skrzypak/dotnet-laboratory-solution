using lab_dotnet_task.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace lab_dotnet_task.Interfaces
{
    public interface IUserService
    {
        public string TestGet();
        public Task<object> Register(RegisterDto dto);
        public Task<object> Login(LoginDto dto);
        public Task<object> Logout();
        public Task<DataDto<UserDataDto>> GetUsers();
    }
}
