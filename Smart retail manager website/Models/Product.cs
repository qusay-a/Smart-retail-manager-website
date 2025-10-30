namespace Smart_retail_manager_website.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public ProductCategories Category {get; set;}
        public decimal UnitPrice { get; set; }
        public int QuantityInStock { get; set; }
        
        public Product(int id, string name, ProductCategories category, decimal price, int stock)
        {
            ProductID = id;
            Name = name;
            Category = category;
            UnitPrice = price;
            QuantityInStock = stock;
        }

        public bool Islowstock()
        {
            return QuantityInStock < 10;
        }
        public decimal CalculateStockValue()
        {
            return UnitPrice * QuantityInStock;
        }
    }
}
