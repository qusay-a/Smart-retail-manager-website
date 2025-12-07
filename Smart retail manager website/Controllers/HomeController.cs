using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Smart_retail_manager_website.Data;
using Smart_retail_manager_website.Models;
using Smart_retail_manager_website.Views.Home;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace Smart_retail_manager_website.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BillRepository _billRepository;
        private readonly List<Product> allProducts = ProductsController.AllProducts;

        public double TaxRate { get; private set; }

        public HomeController(ILogger<HomeController> logger, BillRepository billRepository)
        {
            _logger = logger;
            _billRepository = billRepository;
        }

        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        public IActionResult AddCustomers() => View();

        public IActionResult EditCustomers() => View();

        public async Task<IActionResult> Customers()
        {
            var customers = await _billRepository.GetAllCustomersAsync();
            return View(customers);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                // Optionally, delete all bills for this customer first
                var bills = await _billRepository.GetAllBillsAsync();
                foreach (var bill in bills.Where(b => b.Customer.CustomerID == id))
                {
                    await _billRepository.DeleteBillAsync(bill.BillID);
                }

                // Delete the customer
                await _billRepository.DeleteCustomerAsync(id);

                TempData["Message"] = "Customer and their bills deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                TempData["Error"] = "An error occurred while deleting the customer.";
            }

            return RedirectToAction("Customers");
        }



        [HttpGet]
        public async Task<IActionResult> Bills(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    TempData["Error"] = "Invalid bill ID.";
                    return RedirectToAction("Summary");
                }

                var dbBill = await _billRepository.GetBillDetailsAsync(id.Value);

                // Add this null check here too
                if (dbBill == null)
                {
                    TempData["Error"] = "Bill not found.";
                    return RedirectToAction("Summary");
                }

                return View("BillDetails", dbBill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading bills.");
                TempData["Error"] = "An unexpected error occurred while loading the bills.";
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Bills(
     Customer customer,
     List<int> selectedProducts,
     List<int> productIds,
     List<int> quantities)
        {
            if (!ModelState.IsValid)
                return View("AddCustomers", customer);

            if (selectedProducts == null || selectedProducts.Count == 0)
            {
                TempData["Error"] = "You must select at least one product.";
                return View("AddCustomers", customer);
            }

            if (productIds == null || quantities == null || productIds.Count != quantities.Count)
            {
                TempData["Error"] = "Invalid product selection.";
                return View("AddCustomers", customer);
            }

            try
            {
                int customerId = await _billRepository.InsertCustomerAsync(customer);
                int billId = await _billRepository.InsertBillAsync(customerId, DateTime.Now, TaxRate);

                for (int i = 0; i < productIds.Count; i++)
                {
                    int pid = productIds[i];

                    if (!selectedProducts.Contains(pid))
                        continue;

                    var product = allProducts.FirstOrDefault(p => p.ProductID == pid);
                    if (product == null)
                        continue;

                    int qty = Math.Min(quantities[i], product.QuantityInStock);
                    if (qty <= 0)
                        continue;

                    await _billRepository.InsertBillItemAsync(
                        billId,
                        product.ProductID,
                        product.UnitPrice,
                        qty);

                    product.QuantityInStock -= qty;
                }

                var dbBill = await _billRepository.GetBillDetailsAsync(billId);
                if (dbBill == null)
                {
                    TempData["Error"] = "Bill not found after creation.";
                    return RedirectToAction("Summary");
                }

                return View("BillDetails", dbBill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bill");
                TempData["Error"] = $"An error occurred while creating the bill: {ex.Message}";
                return RedirectToAction("Error");
            }
        }


        // GET: /Home/EditCustomer/5
        [HttpGet]
        public async Task<IActionResult> EditCustomer(int id)
        {
            var customer = await _billRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                TempData["Error"] = "Customer not found.";
                return RedirectToAction("Summary");
            }

            return View(customer);
        }


        // POST: /Home/EditCustomer
        [HttpPost]
        public async Task<IActionResult> EditCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            try
            {
                await _billRepository.UpdateCustomerAsync(customer);
                TempData["Message"] = "Customer updated successfully.";
                return RedirectToAction("Summary");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer");
                TempData["Error"] = "An error occurred while updating the customer.";
                return View(customer);
            }
        }

        // GET: /Home/DeleteBill/5
        [HttpGet]
        public async Task<IActionResult> DeleteBill(int id)
        {
            try
            {
                await _billRepository.DeleteBillAsync(id);
                TempData["Message"] = "Bill deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bill");
                TempData["Error"] = "An error occurred while deleting the bill.";
            }

            return RedirectToAction("Summary");
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