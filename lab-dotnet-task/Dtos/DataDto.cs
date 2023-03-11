namespace lab_dotnet_task.Dtos
{
    public class DataDto<T>
    {
        public ICollection<T> data { get; set; } = new List<T>();
    }
}
