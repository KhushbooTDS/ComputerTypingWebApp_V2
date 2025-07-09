using System.ComponentModel.DataAnnotations;

namespace ComputerTypingWebApp.Models
{
    public class StudentVM
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
        public IFormFile? StudentPicURL { get; set; }
        [Required]
        public IFormFile? IdentityPicURL { get; set; }
        [Required]
        public IFormFile? PhotoIdentity { get; set; }
        [Required]
        public IFormFile? OtherIdentity { get; set; }
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
        [Required]
        public string? English30 { get; set; }
        public string? Hindi30 { get; set; }
        public string? Marathi30 { get; set; }
        public string? English40 { get; set; }
        public string? Hindi40 { get; set; }
        public string? Marathi40 { get; set; }

    }
}
