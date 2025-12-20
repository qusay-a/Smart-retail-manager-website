using System.ComponentModel.DataAnnotations;
namespace Smart_retail_manager_website.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public ProductCategories Category { get; set; }
        [Range(0.01, 1000000, ErrorMessage = "Price must be positive")]
        public decimal UnitPrice { get; set; }
        [Range(0, 1000000, ErrorMessage = "Quantity cannot be negative")]
        public int QuantityInStock { get; set; }

        public Product() { }

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
