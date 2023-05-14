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
    public class ApplicationSettingsControllerTests
    {
        private Mock<DbSet<ApplicationSetting>> _mockSet;
        private Mock<IBudgetContext> _mockContext;

        [SetUp]
        public void Setup()
        {
            var data = GetFakeApplicationSettings().AsQueryable();

            _mockSet = new Mock<DbSet<ApplicationSetting>>();
            _mockSet.As<IAsyncEnumerable<ApplicationSetting>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<ApplicationSetting>(data.GetEnumerator()));

            _mockSet.As<IQueryable<ApplicationSetting>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<ApplicationSetting>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<ApplicationSetting>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<ApplicationSetting>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            _mockContext = new Mock<IBudgetContext>();
            _mockContext.Setup(c => c.ApplicationSetting)
                .Returns(_mockSet.Object);
        }

        [Test]
        public async Task GetApplicationSetting_ReturnsAllSettings()
        {
            var controller = new ApplicationSettingsController(_mockContext.Object);
            var settings = await controller.GetApplicationSetting();

            Assert.NotNull(settings.Value);
            Assert.That(settings.Value.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetApplicationSetting_ReturnsNotFound_WhenSettingDoesNotExist()
        {
            var settingId = 4;
            var controller = new ApplicationSettingsController(_mockContext.Object);

            var result = await controller.GetApplicationSetting(settingId);

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task PutApplicationSetting_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            var settingId = 1;
            var updatedSetting = new ApplicationSetting
            {
                SettingId = 2,
                // other properties...
            };
            var controller = new ApplicationSettingsController(_mockContext.Object);

            var result = await controller.PutApplicationSetting(settingId, updatedSetting);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task PostApplicationSetting_ReturnsSetting_WhenSettingIsValid()
        {
            var newSetting = new ApplicationSetting
            {
                // properties...
            };

            _mockContext.Setup(x => x.ApplicationSetting.Add(newSetting));
            _mockContext.Setup(x => x.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(0));

            var controller = new ApplicationSettingsController(_mockContext.Object);

            var result = await controller.PostApplicationSetting(newSetting);

            Assert.IsNotNull(result);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            var returnValue = createdAtActionResult.Value as ApplicationSetting;
            Assert.IsNotNull(returnValue);
        }

        [Test]
        public async Task DeleteApplicationSetting_ReturnsNoContent_WhenSettingExists()
        {
            var settingId = 1;
            var existingSetting = new ApplicationSetting
            {
                SettingId = settingId,
                // other properties...
            };

            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns(new ValueTask<ApplicationSetting>(existingSetting));
            _mockSet.Setup(m => m.Remove(It.IsAny<ApplicationSetting>())).Verifiable();

            _mockContext.Setup(x => x.ApplicationSetting).Returns(_mockSet.Object);
            _mockContext.Setup(x => x.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(0));

            var controller = new ApplicationSettingsController(_mockContext.Object);

            var result = await controller.DeleteApplicationSetting(settingId);

            Assert.IsInstanceOf<NoContentResult>(result);
            _mockSet.Verify();
        }

        [Test]
        public async Task DeleteApplicationSetting_ReturnsNotFound_WhenSettingDoesNotExist()
        {
            var settingId = 1;

            _mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns(new ValueTask<ApplicationSetting>((ApplicationSetting)null));

            _mockContext.Setup(x => x.ApplicationSetting).Returns(_mockSet.Object);

            var controller = new ApplicationSettingsController(_mockContext.Object);

            var result = await controller.DeleteApplicationSetting(settingId);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task PutApplicationSetting_ReturnsNoContent_WhenIdsMatch()
        {
            var options = new DbContextOptionsBuilder<BudgetContext>()
                .UseInMemoryDatabase(databaseName: "PutApplicationSetting_ReturnsNoContent_WhenIdsMatch")
                .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = new BudgetContext(options))
            {
                context.ApplicationSetting.AddRange(GetFakeApplicationSettings());
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new BudgetContext(options))
            {
                var updatedSetting = new ApplicationSetting
                {
                    SettingId = 1,
                    Name = "Updated Setting",
                    Value = "Updated Value",
                };

                var controller = new ApplicationSettingsController(context);

                var result = await controller.PutApplicationSetting(updatedSetting.SettingId, updatedSetting);

                Assert.IsInstanceOf<NoContentResult>(result);

                // We could also check if the setting has been successfully updated
                var updatedSettingInDb = await context.ApplicationSetting.FindAsync(updatedSetting.SettingId);
                Assert.AreEqual(updatedSetting.Name, updatedSettingInDb.Name);
                Assert.AreEqual(updatedSetting.Value, updatedSettingInDb.Value);
            }
        }

        private static List<ApplicationSetting> GetFakeApplicationSettings()
        {
            return new List<ApplicationSetting>()
            {
                new ApplicationSetting
                {
                    SettingId = 1,
                    Name = "Test Setting 1",
                    Value = "Test Value 1",
                },
                new ApplicationSetting
                {
                    SettingId = 2,
                    Name = "Test Setting 2",
                    Value = "Test Value 2",
                },
                new ApplicationSetting
                {
                    SettingId = 3,
                    Name = "Test Setting 3",
                    Value = "Test Value 3",
                },
            };
        }
    }
}