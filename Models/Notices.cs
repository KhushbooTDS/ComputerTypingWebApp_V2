namespace ComputerTypingWebApp.Models
{
    public class Notices
    {
        public int Id { get; set; }
        public int ToUserId { get; set; }
        public string NoticeText { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
    }
}
