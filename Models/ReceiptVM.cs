namespace ComputerTypingWebApp.Models
{
    public class ReceiptVM
    {
        public int ReceiptNo { get; set; }
        public string? InstituteName { get; set; }
        public string? InstituteAddress { get; set; }
        public DateTime InstallmentDate { get; set; }
        public string? StudentName { get; set; }
        public string? FatherName { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TotalAmountDue { get; set; }
        public decimal BalanceAmountDue { get; set; }
        public string InstitutePhone { get; set; }
        public string School { get; set; }
        public string Section { get; set; }

        public string AmountInWords { get; set; }
    }
}
