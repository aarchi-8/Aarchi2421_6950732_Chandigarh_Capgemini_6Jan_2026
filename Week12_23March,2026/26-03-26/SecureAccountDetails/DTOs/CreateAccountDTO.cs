namespace SecureAccountDetails.Mapping
{
    public class CreateAccountDTO
    {
        public string AccountHolderName { get; set; }

        public string AccountNumber { get; set; }

        public decimal InitialBalance { get; set; }
    }
}
