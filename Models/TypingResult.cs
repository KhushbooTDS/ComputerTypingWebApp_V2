namespace ComputerTypingWebApp.Models
{
    public class TypingResult
    {
        public int TypingResultId { get; set; }
        public int StudentId { get; set; }
        public string? UserName { get; set; }
        public decimal Accuracy { get; set; }
        public DateTime CreateDate { get; set; }
        public int PracticeId { get; set; }
        public int SubjectId { get; set; }
        public int TotalCorrectCharacters { get; set; }
        public int TotalIncorrectCharacters { get; set; }
        public int GrossSpeedPerMinute { get; set; }
        public int NetSpeedPerMinute { get; set; }

    }
}
