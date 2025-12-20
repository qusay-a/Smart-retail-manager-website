using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Smart_retail_manager_website.Models
{
    public class Bill
    {
        [Key]
        public int BillID { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        // FK
        public int CustomerID { get; set; }
        public Customer Customer { get; set; } = null!;

        [Range(0, 1)]
        public decimal TaxRate { get; set; } = 0.05m;

        // link table
        public List<BillProduct> BillProducts { get; set; } = new();

        public decimal CalcSubTotal()
        {
            return BillProducts?.Sum(bp => bp.Price * bp.Quantity) ?? 0m;
        }

        public decimal CalculateTotal()
        {
            var sub = CalcSubTotal();
            return sub + (sub * TaxRate);
        }

    }
}
