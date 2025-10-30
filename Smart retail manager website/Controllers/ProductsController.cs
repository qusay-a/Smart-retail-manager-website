using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smart_retail_manager_website.Models;

namespace Smart_retail_manager_website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return new List<Product>
            {
                new Product(1, "Laptop", ProductCategories.Electronics, 499.99m, 8),
                new Product(2, "Notebook", ProductCategories.Stationery, 1.50m, 100),
                new Product(3, "Milk", ProductCategories.Groceries, 0.95m, 12)
            };
        }
    }
}
