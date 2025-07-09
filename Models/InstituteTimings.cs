namespace ComputerTypingWebApp.Models
{
    public class InstituteTimings
    {
        public int Id { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public int InstituteId { get; set; }
    }
}
