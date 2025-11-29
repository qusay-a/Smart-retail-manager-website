using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Smart_retail_manager_website.Data;
using Smart_retail_manager_website.Models;
using Smart_retail_manager_website.Views.Home;
using System.Diagnostics;
using System.Text.Json;

namespace Smart_retail_manager_website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BillRepository _billRepository;

        private static int nextCustomerId = 1;
        private static int nextBillId = 1;

        private static List<Product> allProducts = ProductsController.AllProducts;

        
        public HomeController(ILogger<HomeController> logger, BillRepository billRepository)
        {
            _logger = logger;
            _billRepository = billRepository;
        }

        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        public IActionResult AddCustomers() => View();

        [HttpGet]
        public IActionResult Bills()
        {
            try
            {
                var bills = GetBillsFromSession();
                return View(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading bills.");
                TempData["Error"] = "An unexpected error occurred while loading the bills.";
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public IActionResult Bills(Customer customer, List<int> selectedProducts, Dictionary<int, int> quantities)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("AddCustomers", customer);
                }

                if (selectedProducts == null || selectedProducts.Count == 0)
                {
                    TempData["Error"] = "You must select at least one product.";
                    return RedirectToAction("AddCustomers");
                }

                customer.CustomerID = nextCustomerId++;
                var bill = new Bill(nextBillId++, customer);

                foreach (var pid in selectedProducts)
                {
                    var product = allProducts.FirstOrDefault(p => p.ProductID == pid);
                    if (product != null)
                    {
                        int qty = quantities.ContainsKey(pid) ? quantities[pid] : 1;

                        if (qty > product.QuantityInStock)
                            qty = product.QuantityInStock;

                        if (qty <= 0)
                            continue;

                        for (int i = 0; i < qty; i++)
                            bill.AddProduct(new Product(product.ProductID, product.Name,
                                product.Category, product.UnitPrice, product.QuantityInStock));

                        product.QuantityInStock -= qty;
                    }
                }

                var existingBills = GetBillsFromSession();
                existingBills.Add(bill);
                SaveBillsToSession(existingBills);

                TempData["SuccessMessage"] = "Bill created successfully!";
                return View("BillDetails", bill);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input while creating bill.");
                ViewBag.ErrorMessage = ex.Message;
                return View("AddCustomers", customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating bill.");
                TempData["Error"] = "An error occurred while creating the bill. Please try again.";
                return RedirectToAction("Error");
            }
        }


        public async Task<IActionResult> Summary()
        {
            var list = await _billRepository.GetBillSummaryAsync();

            var model = new SummaryModel
            {
                Bills = list
            };

            return View(model);
        }
        private List<Bill> GetBillsFromSession()
        {
            var data = HttpContext.Session.GetString("Bills");
            return string.IsNullOrEmpty(data)
                ? new List<Bill>()
                : JsonSerializer.Deserialize<List<Bill>>(data) ?? new List<Bill>();
        }

        private void SaveBillsToSession(List<Bill> bills)
        {
            var json = JsonSerializer.Serialize(bills);
            HttpContext.Session.SetString("Bills", json);
        }

        public IActionResult ForceError()
        {
            try
            {
                throw new Exception("This is a test error to check the error handling system.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ForceError() triggered a test exception.");
                TempData["Error"] = "A test error occurred (for error handling validation).";
                return RedirectToAction("Error");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var errorMessage = exceptionFeature?.Error.Message
                               ?? TempData["Error"]?.ToString()
                               ?? "An unexpected error occurred.";

            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = errorMessage
            };

            return View(model);
        }
    }
}
