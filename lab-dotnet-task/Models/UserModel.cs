namespace lab_dotnet_task.Models
{
    // Model uzytkownika w bazie
    public class UserModel
    {
        public Guid Id { get; set; } 
        public string Username { get; set; }
        public string Password { get; set; }

        // TODO: Odkomentuj w czesci III
        // - jesli zajdzie potrzeba przy pomocy obiektu 'modelBuilder' z pliku DatabaseContext.cs skonfiguruj klucze obce przy pomocy fluentAPI
        virtual public ICollection<MessageModel> FromUserMessages { get; set; }
        virtual public ICollection<MessageModel> ToUserMessages { get; set; }
    }
}
