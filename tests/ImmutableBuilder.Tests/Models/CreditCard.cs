namespace ImmutableBuilder.Tests.Models
{
    public class CreditCard
    {
        private CreditCard() { }

        public string Number { get; private set; }
        
        public CreditCardProcessors Processor { get; private set; }
    }

    public enum CreditCardProcessors
    {
        Visa, MasterCard
    }
}
