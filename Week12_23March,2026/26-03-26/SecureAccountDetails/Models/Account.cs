namespace SecureAccountDetails.Models
{
    public class Account
    {
        public int Id { get; set; }

        public string AccountHolderName { get; set; }

        public string AccountNumber { get; set; }   // 🔴 Sensitive

        public decimal Balance { get; set; }        // 🔴 Sensitive

        public string UserId { get; set; }          // for linking with logged-in user
    }
}
