namespace ComputerTypingWebApp.Models
{
    public class InstituteVM
    {
        public int Id { get; set; }
        public string? InstituteName { get; set; }
        public string? PrincipalName { get; set; }
        public string? InstituteAddress { get; set; }
        public string? InstituteCode { get; set; }
        public string? ContactNo { get; set; }
        public string? Email { get; set; }
        public int? NoOfComputer { get; set; }
        public IFormFile? PrincipalPhotoUrl { get; set; }
        public IFormFile? InstituteSymbolUrl { get; set; }
        public string? Status { get; set; }
    }
}
