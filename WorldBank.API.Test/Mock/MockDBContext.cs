using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldBank.Entities;

namespace WorldBank.API.Test.Mock
{
    internal class MockDBContext
    {
        public Mock<WorldBankDBContext> MockDbContext { get; set; }
        public MockDBContext()
        {
            MockDbContext = new Mock<WorldBankDBContext>();
        }

    }
}
