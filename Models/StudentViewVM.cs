using System.ComponentModel.DataAnnotations;

namespace ComputerTypingWebApp.Models
{
    public class StudentViewVM
    {

        [Required]
        public int StudentId { get; set; }
        public string? LastName { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? FatherName { get; set; }
        [Required]
        public string? MotherName { get; set; }
        [Required]
        public string? MobileNo { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public int? Gender { get; set; }
        [Required]
        public int? Handicap { get; set; }
        [Required]
        public string? PaermentAddress { get; set; }
        [Required]
        public string? School { get; set; }
        [Required]
        public string? Education { get; set; }
        [Required]
        public string? StudentPicURL { get; set; }
        [Required]
        public string? IdentityPicURL { get; set; }
        [Required]
        public string? PhotoIdentity { get; set; }
        [Required]
        public string? OtherIdentity { get; set; }
        [Required]
        public int? IdentityNo { get; set; }
        [Required]
        public string? DOB { get; set; }
        [Required]
        public string? DateAdd { get; set; }
        [Required]
        public string? SelectSub30wpm { get; set; }
        [Required]
        public string? SelectSub40wpm { get; set; }
        [Required]
        public string? Session { get; set; }
        [Required]
        public string? StudentUserName { get; set; }
        [Required]
        public string? StudentPassword { get; set; }

        [Required]
        public string? Status { get; set; }
        public string? SessionList { get; set; }
        [Required]
        public int? StudentType { get; set; }
        [Required]
        public int? SelectedHandicap { get; set; }
        [Required]
        public string? Cast { get; set; }
        [Required]
        public string? UID { get; set; }
        public Students Students { get; set; }
        public Handicap Handicaps { get; set; }
        public Gender Genders { get; set; }
        public string? InstituteName { get; set; }
    }
}
