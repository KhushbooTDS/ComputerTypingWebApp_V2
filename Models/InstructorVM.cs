namespace ComputerTypingWebApp.Models
{
    public class InstructorVM
    {
        public string? InstructorFirstName { get; set; }
        public string? LastName { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? InstructorUserName { get; set; }
        public string? InstructorPassword { get; set; }
        public string? Gender { get; set; }
        public string? PermanentAddress { get; set; }
        public string? Education { get; set; }
        public IFormFile? Identity { get; set; }
        public string? IdentityNo { get; set; }
        public IFormFile? Pic { get; set; }
        public string? Status { get; set; }
        public int? InstituteId { get; set; }
    }
}
