using Microsoft.EntityFrameworkCore;
using Moq;

namespace WorldBank.API.Test.Mock
{
    internal class MockDbSet<T> where T : class
    {
        public Mock<DbSet<T>> mockSet { get; set; }
        public DbSet<T> Object { get { return mockSet.Object; } }
        public MockDbSet(List<T> listData)
        {
            var data = listData.AsQueryable();

            mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

    }
}
