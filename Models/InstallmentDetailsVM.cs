namespace ComputerTypingWebApp.Models
{
    public class InstallmentDetailsVM
    {
        public string SearchSession { get; set; }
        public string SearchGetDate { get; set; }
        public List<Receipts> ReceiptsList { get; set; }
        public List<Receipts> PaidCourseFeeList { get; set; }
        public List<Receipts> TotalAmountPaid { get; set; }

    }
}
