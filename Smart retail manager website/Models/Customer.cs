using System.ComponentModel.DataAnnotations;
namespace Smart_retail_manager_website.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be 2-50 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^\+?\d{7,15}$", ErrorMessage = "Phone must be 7-15 digits (optional +)")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        // an empty constructor cuz why not
        public Customer() { }

        public Customer(int id, string name, string tel, string email)
        {
            CustomerID = id;
            Name = name;
            Phone = tel;
            Email = email;
        }

        public string GetContactSummary()
        {
            return $"Customer name is {Name}, and their contact is {Phone} and {Email}";
        }



    }
}
