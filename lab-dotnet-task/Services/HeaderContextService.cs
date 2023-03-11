using lab_dotnet_task.Interfaces;
using System.Security.Claims;

namespace lab_dotnet_task.Services
{
    // Sluzy do wszczykiwania obiektu HttpContext
    public class HeaderContextService : IHeaderContextService
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public HeaderContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpContext? GetHttpContext()
        {
            return _httpContextAccessor.HttpContext;
        }

        private ClaimsPrincipal GetUser()
        {
            return GetHttpContext()!.User;
        }

        public Guid GetUserId()
        {
            var claim = GetUser().FindFirstValue(ClaimTypes.NameIdentifier);
            return new Guid(claim);
        }
    }
}
