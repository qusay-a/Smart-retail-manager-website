using Microsoft.AspNetCore.Mvc;
using Smart_retail_manager_website.Models;
using System.Collections.Generic;

namespace Smart_retail_manager_website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public static List<Product> AllProducts = new List<Product>
    {
            new Product(1, "Laptop",   ProductCategories.Electronics, 499.99m, 8),
            new Product(2, "Notebook", ProductCategories.Stationery,  1.50m,   100),
            new Product(3, "Milk",     ProductCategories.Groceries,   0.95m,   12)
    };


        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return AllProducts;
        }
    }
}
