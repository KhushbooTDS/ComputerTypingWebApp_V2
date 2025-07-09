namespace ComputerTypingWebApp.Models
{
    public class UserLogins
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Login { get; set; }
        public DateTime LogOut { get; set; }
    }
}
