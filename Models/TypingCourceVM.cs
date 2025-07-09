using System.Collections.Generic;

namespace ComputerTypingWebApp.Models
{
    public class TypingCourceVM
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string PracticeName { get; set; }
        public string SubjectName { get; set; }
        public string PracticeData { get; set; }
        public List<TypingCourceVM> ListTypingCource { get; set; }
        public List<coursepractices> Listcoursepractices { get; set; }
        public List<Course> ListCourse { get; set; }
        public List<Subject> ListSubject { get; set; }
        public List<Students> ListOfSubject { get; set; }
        public List<CoursesUpload> ListCoursesUpload { get; set; }

    }
}
