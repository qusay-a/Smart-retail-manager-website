using System.ComponentModel.DataAnnotations;
﻿namespace Smart_retail_manager_website.Models
{
    public class Bill
    {
        [Key]
        public int BillID { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Customer details are required")]
        public Customer Customer { get; set; }
        [Required(ErrorMessage = "At least one item is required")]
        public List<Product> Items { get; set; }
        [Range(0, 1, ErrorMessage = "Tax rate must be between 0 and 1")]
        public decimal TaxRate { get; set; } = 0.05m;

        public Bill()
        {
            Date = DateTime.Now;
            Items = new List<Product>();
        }

        public Bill(int billId, Customer customer)
        {
            BillID = billId;
            Customer = customer;
            Date = DateTime.Now;
            Items = new List<Product>();
        }

        public void AddProduct(Product item)
        {
            Items.Add(item);
        }

        public decimal CalcSubTotal()
        {
            //.Sum() is a linq(language integrated query) metehod.
            // p => p.UnitPrice 
            return Items.Sum(p => p.UnitPrice);
        }

        public decimal CalculateTotal()
        {
            decimal subtotal = CalcSubTotal();
            decimal tax = subtotal * TaxRate;
            decimal total = tax + subtotal;
            return total;
        }
        public int GetTotalItems()
        {
            return Items.Count;
        }

    }
}
