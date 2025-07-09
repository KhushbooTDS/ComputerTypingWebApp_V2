namespace ComputerTypingWebApp.Models
{
    public class CoursesUpload
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int SubjectId { get; set; }
        public int PracticeId { get; set; }
        public string PracticeData { get; set; }
        public int UserId { get; set; }
        public int InstituteId { get; set; }
        public string UniCodePracticeData { get; set; }
    }
}
