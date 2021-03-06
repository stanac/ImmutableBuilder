using ImmutableBuilder.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ImmutableBuilder.Tests
{
    public class ImmutableBuilderTests
    {
        [Fact]
        public void CanSetProperties()
        {
            Person p = BuildTestPerson();

            Assert.Equal(29, p.Age);
            Assert.Equal("Kovalski", p.Name);
        }

        [Fact]
        public void CanSetIReadOnlyList()
        {
            CreditCard cc1 = new Builder<CreditCard>()
                .Set(x => x.Number, "1234")
                .Set(x => x.Processor, CreditCardProcessors.Visa)
                .Build();

            CreditCard cc2 = new Builder<CreditCard>()
                .Set(x => x.Number, "xzya")
                .Set(x => x.Processor, CreditCardProcessors.MasterCard)
                .Build();

            IReadOnlyList<CreditCard> ccs = new List<CreditCard> { cc1, cc2 };

            Person p = new Builder<Person>()
                .Set(x => x.CreditCards, ccs)
                .Build();

            Assert.NotNull(p.CreditCards);
            Assert.NotEmpty(p.CreditCards);
            Assert.Equal(2, p.CreditCards.Count);
            Assert.Equal("1234", p.CreditCards.First().Number);
        }

        [Fact]
        public void CanSetIEnumerable()
        {
            Account account1 = new Builder<Account>()
                .Set(x => x.AccountNumber, "123")
                .Set(x => x.IsSavingsAccount, true)
                .Build();

            Account account2 = new Builder<Account>()
                .Set(x => x.AccountNumber, "987")
                .Set(x => x.IsSavingsAccount, false)
                .Build();

            IEnumerable<Account> accounts = new[] { account1, account2 };

            Person p = new Builder<Person>()
                .Set(x => x.Accounts, accounts)
                .Build();

            Assert.NotNull(p.Accounts);
            Assert.NotEmpty(p.Accounts);
            Assert.Equal(2, p.Accounts.Count());
            Assert.Equal("987", p.Accounts.Last().AccountNumber);
        }

        [Fact]
        public void CanCloneObject()
        {
            Person p1 = BuildTestPerson();

            Builder<Person> newBuilder = Builder<Person>.FromObject(p1);

            Person p2 = newBuilder.Build();

            Assert.NotNull(p1);
            Assert.NotNull(p2);
            Assert.NotEqual(p1, p2);
            Assert.Equal(p1.Age, p2.Age);
            Assert.Equal(p1.Name, p2.Name);
            Assert.False(object.ReferenceEquals(p1, p2));

            p2 = Builder<Person>.CloneShallow(p1);

            Assert.NotNull(p1);
            Assert.NotNull(p2);
            Assert.NotEqual(p1, p2);
            Assert.Equal(p1.Age, p2.Age);
            Assert.Equal(p1.Name, p2.Name);
        }

        [Fact]
        public void CanChangeObject()
        {
            Person p1 = BuildTestPerson();
            Person p2 = Builder<Person>.Change(p1, m => m.Age, 11);

            Assert.NotNull(p1);
            Assert.NotNull(p2);
            Assert.NotEqual(p1, p2);
            Assert.Equal(11, p2.Age);
            Assert.NotEqual(p1.Age, p2.Age);
            Assert.Equal(p1.Name, p2.Name);
        }

        [Fact]
        public void BuilderWithRequiredProps_ListOfSetPropertiesUpdates()
        {
            var builder = new Builder<Account>(throwExceptionOnBuildIfNotAllPropsAreSet: true);

            Assert.False(builder.AreAllPropertiesSet);
            Assert.Empty(builder.SetPropertiesNames);
            Assert.Equal(2, builder.NotSetPropertiesNames.Count());

            builder.Set(x => x.AccountNumber, "123");
            Assert.False(builder.AreAllPropertiesSet);
            Assert.NotEmpty(builder.SetPropertiesNames);
            Assert.Equal("AccountNumber", builder.SetPropertiesNames.Single());
            Assert.Single(builder.NotSetPropertiesNames);

            builder.Set(x => x.IsSavingsAccount, true);
            Assert.True(builder.AreAllPropertiesSet);
            Assert.Equal(2, builder.SetPropertiesNames.Count());
            Assert.Contains("AccountNumber", builder.SetPropertiesNames);
            Assert.Contains("IsSavingsAccount", builder.SetPropertiesNames);
            Assert.Empty(builder.NotSetPropertiesNames);
        }

        [Fact]
        public void BuilderFromModel_AllPropsAreSet()
        {
            Person model = BuildTestPerson();

            var builder = Builder<Person>.FromObject(model);
            Assert.True(builder.AreAllPropertiesSet);
        }

        [Fact]
        public void BuildThrowingExceptionIfNotAllPropertiesAreSetInBuild()
        {
            Func<Account> test = () => new Builder<Account>(throwExceptionOnBuildIfNotAllPropsAreSet: true)
                .Set(x => x.AccountNumber, "123")
                //.Set(x => x.IsSavingsAccount, false)
                .Build();

            bool error = false;
            try
            {
                test();
            }
            catch (InvalidOperationException)
            {
                error = true;
            }

            Assert.True(error, "Exception is throw");
        }

        [Fact]
        public void AllInOneSample()
        {
            Account account1 = new Builder<Account>()
                .Set(x => x.AccountNumber, "123")
                .Set(x => x.IsSavingsAccount, true)
                .Build();

            Account account2 = new Builder<Account>()
                .Set(x => x.AccountNumber, "987")
                .Set(x => x.IsSavingsAccount, false)
                .Build();

            CreditCard cc1 = new Builder<CreditCard>()
                .Set(x => x.Number, "1234")
                .Set(x => x.Processor, CreditCardProcessors.Visa)
                .Build();

            CreditCard cc2 = new Builder<CreditCard>()
                .Set(x => x.Number, "xzya")
                .Set(x => x.Processor, CreditCardProcessors.MasterCard)
                .Build();

            IEnumerable<Account> accounts = new[] { account1, account2 };

            IReadOnlyList<CreditCard> ccs = new List<CreditCard> { cc1, cc2 };

            Person p = new Builder<Person>()
                .Set(x => x.Age, 29)
                .Set(x => x.Name, "Kovalski")
                .Set(x => x.CreditCards, ccs)
                .Set(x => x.Accounts, accounts)
                .Build();
        }

        private Person BuildTestPerson()
        {
            return new Builder<Person>()
                .Set(x => x.Age, 29)
                .Set(x => x.Name, "Kovalski")
                .Build();
        }
    }
}
