namespace ComputerTypingWebApp.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public int? UserId { get; set; }
        public int? CourseId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMode { get; set; }
        public string ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime NextInstallmentDate { get; set; }
        public int SessionId { get; set; }
    }
}
