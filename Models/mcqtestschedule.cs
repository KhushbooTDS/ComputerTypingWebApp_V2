namespace ComputerTypingWebApp.Models
{
    public class mcqtestschedule
    {
        public int Id { get; set; } 
        public DateTime TestDate { get; set; }
        public int SubjectId { get; set; }
        public int SectionId { get; set; }
        public int NoOfQuest { get; set; }
        public int EachQuesMark { get; set; }
        public int PassingMarks { get; set; }
        public decimal TestDuration { get; set; }
        public int InstituteId { get; set; }
    }
}
