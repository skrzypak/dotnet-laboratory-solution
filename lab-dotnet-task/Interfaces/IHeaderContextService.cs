namespace lab_dotnet_task.Interfaces
{
    // Potrzebny do wszczykiwania obiektu HttpContext
    public interface IHeaderContextService
    {
        public HttpContext? GetHttpContext();
        public Guid GetUserId();

    }
}
