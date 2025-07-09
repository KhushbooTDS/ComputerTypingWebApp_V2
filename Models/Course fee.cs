namespace ComputerTypingWebApp.Models
{
    public class Coursefee
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string StudentType { get; set; }
        public double Fees { get; set;}
        public int Instituteid { get; set; }
        public int subjectid { get; set;}
    }
}
