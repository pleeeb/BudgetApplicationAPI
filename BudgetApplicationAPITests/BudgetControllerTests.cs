using BudgetApplicationAPI.Controllers;
using BudgetApplicationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BudgetApplicationAPITests
{
    public class BudgetControllerTests
    {
        private Mock<DbSet<Budget>> _mockSet;
        private Mock<IBudgetContext> _mockContext;

        [SetUp]
        public void Setup()
        {
            var data = GetFakeBudgetList().AsQueryable();

            _mockSet = new Mock<DbSet<Budget>>();
            _mockSet.As<IAsyncEnumerable<Budget>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Budget>(data.GetEnumerator()));

            _mockSet.As<IQueryable<Budget>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<Budget>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<Budget>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<Budget>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            _mockContext = new Mock<IBudgetContext>();
            _mockContext.Setup(c => c.Budget)
                .Returns(_mockSet.Object);
        }

        [Test]
        public async Task GetAllBudgetsTest()
        {
            var controller = new BudgetsController(_mockContext.Object);
            var budgets = await controller.GetBudget();

            Assert.NotNull(budgets.Value);
            Assert.That(budgets.Value.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetBudget_ReturnsNotFound_WhenBudgetDoesNotExist()
        {
            var budgetId = 4;
            var controller = new BudgetsController(_mockContext.Object);

            var result = await controller.GetBudget(budgetId);

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task PutBudget_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            var budgetId = 1;
            var updatedBudget = new Budget
            {
                BudgetId = 2,
                // other properties...
            };
            var controller = new BudgetsController(_mockContext.Object);

            var result = await controller.PutBudget(budgetId, updatedBudget);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task PostBudget_ReturnsBudget_WhenBudgetIsValid()
        {
            var newBudget = new Budget
            {
                // properties...
            };

            _mockContext.Setup(x => x.Budget.Add(newBudget));
            _mockContext.Setup(x => x.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(0));

            var controller = new BudgetsController(_mockContext.Object);

            var result = await controller.PostBudget(newBudget);

            Assert.IsNotNull(result);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            var returnValue = createdAtActionResult.Value as Budget;
            Assert.IsNotNull(returnValue);
        }

        [Test]
        public async Task DeleteBudget_ReturnsNoContent_WhenBudgetExists()
        {
            var budgetId = 1;
            var existingBudget = new Budget
            {
                BudgetId = budgetId,
                // other properties...
            };

            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns(new ValueTask<Budget>(existingBudget));

            _mockContext.Setup(x => x.Budget).Returns(_mockSet.Object);

            var controller = new BudgetsController(_mockContext.Object);

            var result = await controller.DeleteBudget(budgetId);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteBudget_ReturnsNotFound_WhenBudgetDoesNotExist()
        {
            var budgetId = 4;
            var controller = new BudgetsController(_mockContext.Object);

            var result = await controller.DeleteBudget(budgetId);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        private static List<Budget> GetFakeBudgetList()
        {
            return new List<Budget>()
            {
                new Budget
                {
                    BudgetId = 1,
                    UserId = 1,
                    Name = "Holiday",
                    Description = "Holiday budget",
                    Amount = (decimal?)2000.00,
                    StartDate = DateTime.Now,
                    EndDate = (DateTime.Now).AddMonths(2),
                },
                new Budget
                {
                    BudgetId = 2,
                    UserId = 1,
                    Name = "Christmas",
                    Description = "Christmas budget",
                    Amount = (decimal?)1500.00,
                    StartDate = DateTime.Now,
                    EndDate = (DateTime.Now).AddMonths(6),
                },
                new Budget
                {
                    BudgetId = 3,
                    UserId = 2,
                    Name = "Holiday",
                    Description = "Holiday budget",
                    Amount = (decimal?)8000.00,
                    StartDate = DateTime.Now,
                    EndDate = (DateTime.Now).AddMonths(1),
                },
            };
        }
    }
}