namespace Smart_retail_manager_website.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
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
