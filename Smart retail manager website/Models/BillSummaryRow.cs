namespace Smart_retail_manager_website.Models
{
    using System;
    public class BillSummaryRow
    {
        public int BillID { get; set; }
        public string Cname { get; set; } = String.Empty;
        public DateTime DateOfInvoice { get; set; }
        public decimal LineTotal { get; set; }

    }
}
