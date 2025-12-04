namespace Smart_retail_manager_website.Models
{
    public class BillItemRow
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal UnitPrice { get; internal set; }
    }
}
