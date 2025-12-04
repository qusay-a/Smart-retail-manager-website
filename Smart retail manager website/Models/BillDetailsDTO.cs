using System;
using System.Collections.Generic;

namespace Smart_retail_manager_website.Models
{
    public class BillDetailsDTO
    {
        public int BillID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<BillItemRow> Items { get; set; } = new List<BillItemRow>();
    }
}
