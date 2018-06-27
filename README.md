# ImmutableBuilder
.NET Standard library for building immutable models

## Install
from nuget, requires NETStandard 2.0

```
PM> Install-Package ImmutableBuilder
```
or
```
dotnet add package ImmutableBuilder
```

## Use

**Sample models**

```csharp
public class Account
{
    private Account() { }

    public string AccountNumber { get; private set; }

    public bool IsSavingsAccount { get; private set; }
}

public class CreditCard
{
    public string Number { get; private set; }
    
    public CreditCardProcessors Processor { get; private set; }
}

public enum CreditCardProcessors
{
    Visa, MasterCard
}

public class Person
{
    private Person() { }

    public string Name { get; private set; }

    public int Age { get; private set; }

    public IReadOnlyList<CreditCard> CreditCards { get; private set; }

    public IEnumerable<Account> Accounts { get; private set; }
}
```

**Building models**

```csharp
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
```

**Cloning model by value**
```csharp
Person p1 = p = new Builder<Person>()
    .Set(x => x.Age, 29)
    .Set(x => x.Name, "Kovalski")
    .Build();

Builder<Person> newBuilder = Builder<Person>.FromObject(p1);
// you can change here properties of new object with newBuilder.Set method

Person p2 = newBuilder.Build();
```

**Changing properties**
```csharp
// This method is creating new object form existing one and setting
// new property value to new object, existing object is not changed

Person p1 = p = new Builder<Person>()
    .Set(x => x.Age, 29)
    .Set(x => x.Name, "Kovalski")
    .Build();

Person p2 = Builder<Person>.Change(p1, m => m.Age, 11);
```