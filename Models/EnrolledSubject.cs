namespace ComputerTypingWebApp.Models
{
    public class EnrolledSubject
    {
        public int EnrolledSubjectId { get; set; }
        public int GRNumber { get; set; }
        public string? SubjectName { get; set; }
        public string? UserName { get; set; }
        public int CreateBy { get; set; }
        public int instituteid { get; set; }
    }
}
