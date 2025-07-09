namespace ComputerTypingWebApp.Models
{
    public class SaveProgressModel
    {
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public int PracticeId {  get; set; }
        public int SubjectId {  get; set; }
        public int TotalCorrectCharacters { get; set; }
        public int TotalIncorrectCharacters { get; set; }
        public int GrossSpeedPerMinute {  get; set; }
        public int NetSpeedPerMinute {  get; set; }
    }
}
