namespace ComputerTypingWebApp.Models
{
    public class mcqquestions
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int SectionId { get; set; }
        public int InstituteId { get; set; }
        public string QuesTitle { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string CorrectAnswer { get; set; }
        public DateTime DateUploaded { get; set; }
        public bool IsDeleted { get; set; }
    }
}
