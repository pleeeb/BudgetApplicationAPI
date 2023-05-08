    using BudgetApplicationAPI.Controllers;
    using BudgetApplicationAPI.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Query.Internal;
    using Microsoft.EntityFrameworkCore.Storage;

    namespace BudgetApplicationAPITests
    {
        [TestFixture]
        public class UsersControllerTests
        {
            private UsersController? _controller;
            private Mock<IBudgetContext>? _mockContext;

            [SetUp]
            public void Setup()
            {
                _mockContext = new Mock<IBudgetContext>();
                _controller = new UsersController(_mockContext.Object);
            }

            [Test]
            public async Task GetUsers_ReturnsAListOfUsers()
            {
                // Arrange
                var options = new DbContextOptionsBuilder<BudgetContext>()
                    .UseInMemoryDatabase(databaseName: "BudgetDatabase", new InMemoryDatabaseRoot())
                    .Options;

                var context = new BudgetContext(options);
                var users = new List<User>
                {
                    new User { UserId = 1, Username = "user1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", PasswordHash = "abc123" },
                    new User { UserId = 2, Username = "user2", Email = "user2@example.com", FirstName = "Jane", LastName = "Doe", PasswordHash = "def456" }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();

                ActionResult<IEnumerable<User>> result;
                // Act
                var controller = new UsersController(context);
                result = await controller.GetUsers();

                // Assert
                Assert.IsNotNull(result.Value);
                Assert.AreEqual(2, result.Value?.Count());
            }

            [Test]
            public async Task GetUser_ReturnsUserById()
            {
                // Arrange
                int userId = 1;
                var user = new User { UserId = userId, Username = "user1", Email = "user1@example.com" };

                _mockContext?.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync(user);

                // Act
                var result = await _controller!.GetUser(userId);

                // Assert
                Assert.NotNull(result.Value);
                Assert.AreEqual(userId, result.Value?.UserId);
                Assert.AreEqual(user, result.Value);
            }

            [Test]
            public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
            {
                // Arrange
                int userId = 1;
                _mockContext?.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync(null as User);

                // Act
                var result = await _controller!.GetUser(userId);

                // Assert
                Assert.IsInstanceOf<NotFoundResult>(result.Result);
            }

        [Test]
        public async Task PutUser_UpdatesUserAndReturnsNoContent()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BudgetContext>()
                .UseInMemoryDatabase(databaseName: "BudgetDatabase")
                .Options;
            var context = new BudgetContext(options);

            var userId = 1;
            var existingUser = new User { UserId = userId, Username = "user1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", PasswordHash = "abc123" };
            await context.Users.AddAsync(existingUser);
            await context.SaveChangesAsync();

            var updatedUser = new User { UserId = userId, Username = "user1", Email = "user1@example.com", FirstName = "Jane", LastName = "Doe", PasswordHash = "abc123" };

            var controller = new UsersController(context);

            // Act
            var result = await controller.PutUser(userId, updatedUser);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);

            var savedUser = await context.Users.FindAsync(userId);
            Assert.AreEqual(updatedUser.FirstName, savedUser?.FirstName);
        }

        [Test]
            public async Task PutUser_ReturnsBadRequest_WhenUserIdsDoNotMatch()
            {
                // Arrange
                int userId = 1;
                var user = new User { UserId = 2, Username = "user1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", PasswordHash = "abc123" };

                // Act
                var result = await _controller!.PutUser(userId, user);

                // Assert
                Assert.IsInstanceOf<BadRequestResult>(result);
            }

        [Test]
        public async Task PutUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 1;
            var user = new User { UserId = userId, Username = "user1", Email = "user1@example.com" };
            _mockContext?.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync(null as User);

            // Act
            var result = await _controller!.PutUser(userId, user);

            // Assert
            _mockContext?.Verify(x => x.Users.FindAsync(userId), Times.Once);
            _mockContext?.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
            public async Task PostUser_CreatesNewUserAndReturnsCreatedAtAction()
            {
                // Arrange
                var user = new User { UserId = 1, Username = "user1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", PasswordHash = "abc123" };

                _mockContext?.Setup(x => x.Users.Add(user));
                _mockContext?.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

                // Act
                var result = await _controller!.PostUser(user);

                // Assert
                _mockContext?.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
                Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            }

            [Test]
            public async Task DeleteUser_RemovesUserAndReturnsNoContent()
            {
                // Arrange
                int userId = 1;
                var user = new User { UserId = userId, Username = "user1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", PasswordHash = "abc123" };

                _mockContext?.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync(user);
                _mockContext?.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

                // Act
                var result = await _controller!.DeleteUser(userId);

                // Assert
                _mockContext?.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
                Assert.IsInstanceOf<NoContentResult>(result);
            }
        }
    }