using BudgetApplicationAPI.Controllers;
using BudgetApplicationAPI.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net.Sockets;

namespace BudgetApplicationAPITests
{
    public class Tests
    {
        private Mock<DbSet<User>> _mockSet;
        private Mock<IBudgetContext> _mockBudgetContext;

        [SetUp]
        public void Setup()
        {
            // Arrange
            var data = GetFakeUserList().AsQueryable();

            _mockSet = new Mock<DbSet<User>>();
            _mockSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<User>(data.GetEnumerator()));

            _mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            _mockBudgetContext = new Mock<IBudgetContext>();
            _mockBudgetContext.Setup(x => x.Users)
                .Returns(_mockSet.Object);
        }

        [Test]
        public async Task GetAllUsersTest()
        {
            // Act
            var service = new UsersController(_mockBudgetContext.Object);
            var users = await service.GetUsers();

            // Assert
            Assert.NotNull(users.Value);
            Assert.That(users.Value.Count(), Is.EqualTo(3));
        }

        private static List<User> GetFakeUserList()
        {
            return new List<User>()
            {
                new User
                {
                    UserId = 1,
                    Username = "Test",
                    Email = "testemail@hotmail.com",
                    PasswordHash = "abc1",
                    FirstName = "Test",
                    LastName = "TestLast",
                    CreatedOn = DateTime.Now,
                    LastLoginOn = DateTime.Now,
                    IsActive = true,
                },
                new User
                {
                    UserId = 2,
                    Username = "Test2",
                    Email = "test2email@hotmail.com",
                    PasswordHash = "def2",
                    FirstName = "Test2",
                    LastName = "Test2Last",
                    CreatedOn = DateTime.Now,
                    LastLoginOn = DateTime.Now,
                    IsActive = true,
                },
                new User
                {
                    UserId = 3,
                    Username = "Test3",
                    Email = "test3email@hotmail.com",
                    PasswordHash = "abc3",
                    FirstName = "Test3",
                    LastName = "Test3Last",
                    CreatedOn = DateTime.Now,
                    LastLoginOn = DateTime.Now,
                    IsActive = true,
                },
            };
        }
    }
}