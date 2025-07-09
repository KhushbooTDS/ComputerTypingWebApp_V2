namespace ComputerTypingWebApp.Models
{
    public class FeeInstallmentVM
    {
        public int Id { get; set; }
        public DateTime InstallmentDate { get; set; }
        public string StudentType { get; set; }
        public string StudentUserName { get; set; }
        public string SubjectIds { get; set; }
        public string SubjectName { get; set; }
        public decimal TotalAmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceAmountDue { get; set; }
        public string PaymentMadeBy { get; set; }
        public string? ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
