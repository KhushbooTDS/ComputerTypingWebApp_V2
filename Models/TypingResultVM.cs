namespace ComputerTypingWebApp.Models
{
    public class TypingResultVM
    {
            public int StudentId { get; set; }
            public string? SubjectName { get; set; }
            public string? UserName { get; set; }
            public int PracticeId { get; set; }
            public string? PracticName { get; set; }
            public string? Accurecy { get; set; }
            public int SubjectId { get; set; }
            public int TotalCorrectCharacters { get; set; }
            public int TotalIncorrectCharacters { get; set; }
            public int GrossSpeedPerMinute { get; set; }
            public int NetSpeedPerMinute { get; set; }
    }
}
