using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ImmutableBuilder.Tests
{
    public class ConstructorDelegateTests
    {
        [Fact]
        public void ForObjectWithoutParametarlessConstructorExceptionIsThrown()
        {
            bool error = false;
            try
            {
                ConstructorDelegate.GetConstructor<Models.NoValidCtor>();
            }
            catch (Exception)
            {
                error = true;
            }

            Assert.True(error);
        }
    }


}
