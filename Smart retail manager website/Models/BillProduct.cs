namespace Smart_retail_manager_website.Models
{
    public class BillProduct
    {
        public int BillProductID { get; set; }

        public int BillID { get; set; }
        public Bill Bill { get; set; }

        public int ProductID { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
