namespace ImmutableBuilder.Tests.Models
{
    public class Account
    {
        private Account() { }

        public string AccountNumber { get; private set; }

        public bool IsSavingsAccount { get; private set; }
    }
}
