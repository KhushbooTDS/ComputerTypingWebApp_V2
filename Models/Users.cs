using System.ComponentModel.DataAnnotations;

namespace ComputerTypingWebApp.Models
{
    public class Users
    {
        
        public int? Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool? IsActive { get; set; }
        public int? RoleId { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Institute is required.")]
        public int? InstituteId { get; set; }
    }
}
