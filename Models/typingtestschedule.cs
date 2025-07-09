namespace ComputerTypingWebApp.Models
{
    public class typingtestschedule
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int SectionId { get; set; }
        public int SpeedPracticeUploadId { get; set; }
        public  int InstituteId { get; set; }
        public DateTime TestDate { get; set; }
    }
}
