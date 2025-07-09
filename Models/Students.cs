namespace ComputerTypingWebApp.Models
{
    public class Students
    {
        public int Id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? MobileNumber { get; set; }
        public string? EmailId { get; set; }
        public int? Gender { get; set; }
        public int? Handicap { get; set; }
        public string? PaermentAddress { get; set; }
        public string? School { get; set; }
        public string? Education { get; set; }
        public string? PhotoIdentityURL { get; set; }
        public string? OtherIdentityURL { get; set; }
        public int? IdentityNo { get; set; }
        public string? DOB { get; set; }
        public string? DateAdd { get; set; }
        public string? SelectSub30wpm { get; set; }
        public string? SelectSub40wpm { get; set; }
        public string? Session { get; set; }

        public string? StudentUserName { get; set; }
        public string? StudentPassword { get; set; }
        public string? StudentPicURL { get; set; }
        public string? IdentityPicURL { get; set; }
        public string? Status { get; set; }
        public int? StudentType { get; set; }
        public string? Cast { get; set; }
        public string? UID { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
