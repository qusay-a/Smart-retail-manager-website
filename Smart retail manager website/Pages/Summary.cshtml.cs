using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Smart_retail_manager_website.Data;
using Smart_retail_manager_website.Models;

namespace Smart_retail_manager_website.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly BillRepository _repo;

        public List<BillSummaryRow> Bills { get; set; } = new();

        public SummaryModel(BillRepository repo)
        {
            _repo = repo;
        }

        public async Task OnGetAsync()
        {
            Bills = await _repo.GetBillSummaryAsync();
        }
    }
}