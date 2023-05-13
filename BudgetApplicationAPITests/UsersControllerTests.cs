using BudgetApplicationAPI.Controllers;
using BudgetApplicationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        [Test]
        public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 4;
            var service = new UsersController(_mockBudgetContext.Object);

            // Act
            var result = await service.GetUser(userId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task PutUser_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var userId = 1;
            var updatedUser = new User
            {
                UserId = 2,
                Username = "UpdatedTest",
                Email = "updatedemail@hotmail.com",
                PasswordHash = "updatedhash",
                FirstName = "Updated",
                LastName = "UpdatedLast",
                CreatedOn = DateTime.Now,
                LastLoginOn = DateTime.Now,
                IsActive = true,
            };
            var service = new UsersController(_mockBudgetContext.Object);

            // Act
            var result = await service.PutUser(userId, updatedUser);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task PutUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 4;
            var updatedUser = new User
            {
                UserId = userId,
                Username = "UpdatedTest",
                Email = "updatedemail@hotmail.com",
                PasswordHash = "updatedhash",
                FirstName = "Updated",
                LastName = "UpdatedLast",
                CreatedOn = DateTime.Now,
                LastLoginOn = DateTime.Now,
                IsActive = true,
            };
            var service = new UsersController(_mockBudgetContext.Object);

            // Act
            var result = await service.PutUser(userId, updatedUser);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task PostUser_ReturnsUser_WhenUserIsValid()
        {
            // Arrange
            var newUser = new User
            {
                Username = "NewTest",
                Email = "newtest@hotmail.com",
                PasswordHash = "newhash",
                FirstName = "New",
                LastName = "NewLast",
                CreatedOn = DateTime.Now,
                LastLoginOn = DateTime.Now,
                IsActive = true,
            };

            _mockBudgetContext.Setup(x => x.Users.Add(newUser));
            _mockBudgetContext.Setup(x => x.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(0));

            var service = new UsersController(_mockBudgetContext.Object);

            // Act
            var result = await service.PostUser(newUser);

            // Assert
            Assert.IsNotNull(result);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            var returnValue = createdAtActionResult.Value as User;
            Assert.IsNotNull(returnValue);
        }

        [Test]
        public async Task DeleteUser_ReturnsNoContent_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var existingUser = new User
            {
                UserId = userId,
                // other properties...
            };

            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns(new ValueTask<User>(existingUser));

            _mockBudgetContext.Setup(x => x.Users).Returns(_mockSet.Object);

            var service = new UsersController(_mockBudgetContext.Object);

            // Act
            var result = await service.DeleteUser(userId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 4;
            var service = new UsersController(_mockBudgetContext.Object);

            // Act
            var result = await service.DeleteUser(userId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
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