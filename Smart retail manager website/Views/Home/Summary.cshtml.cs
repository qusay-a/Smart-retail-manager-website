using System.Collections.Generic;
using Smart_retail_manager_website.Models;

namespace Smart_retail_manager_website.Views.Home
{
    
    public class SummaryModel
    {
        public List<BillSummaryRow> Bills { get; set; } = new();    
    }
}
