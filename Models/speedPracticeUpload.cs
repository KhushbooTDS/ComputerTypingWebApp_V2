namespace ComputerTypingWebApp.Models
{
    public class speedPracticeUpload
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int SubjectId { get; set; }
        public int PracticeId { get; set; }
        public string FilePath { get; set; }
        public int UserId { get; set; }
        public int InstituteId { get; set; }
        public string FilToken { get; set; }
        public int sectionid { get; set; }

        public DateTime DateUploaded { get; set; } 
        public string FileName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
