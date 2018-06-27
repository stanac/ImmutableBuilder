using System.Collections.Generic;

namespace ImmutableBuilder.Tests.Models
{
    public class Person
    {
        private Person() { }

        public string Name { get; private set; }

        public int Age { get; private set; }

        public IReadOnlyList<CreditCard> CreditCards { get; private set; }

        public IEnumerable<Account> Accounts { get; private set; }
    }
}
