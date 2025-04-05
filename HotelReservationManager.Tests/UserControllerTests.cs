using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelReservationManager.Controllers;
using HotelReservationManager.Data;
using HotelReservationManager.Data.Models;
using HotelReservationManager.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationManager.Tests
{
    public class UserControllerTests
    {
        private Mock<ApplicationDbContext> _mockContext;
        private Mock<UserManager<User>> _mockUserManager;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null);

            _controller = new UserController(_mockContext.Object, _mockUserManager.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose the controller after each test
            _controller.Dispose();
        }

        [Test]
        public async Task Edit_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var invalidId = "non-existent-id";

            _mockContext.Setup(c => c.Users.FindAsync(invalidId)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Edit(invalidId);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.NotNull(notFoundResult);
        }

        [Test]
        public async Task Edit_ShouldReturnView_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = "1", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "john.doe@example.com" };
            var userVM = new EditUserViewModel
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                EGN = "1234567890"
            };

            _mockContext.Setup(c => c.Users.FindAsync("1")).ReturnsAsync(user);

            // Act
            var result = await _controller.Edit("1");

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            var model = viewResult.Model as EditUserViewModel;
            Assert.AreEqual(userVM.Id, model.Id);
        }
        [Test]
        public async Task DeleteConfirmed_ShouldRemoveUserAndRedirect()
        {
            // Arrange
            var user = new User { Id = "1", FirstName = "John", LastName = "Doe" };

            _mockContext.Setup(c => c.Users.FindAsync("1")).ReturnsAsync(user);

            _mockContext.Setup(c => c.Users.Remove(It.IsAny<User>())).Verifiable();

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteConfirmed("1");

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);

            _mockContext.Verify(c => c.Users.Remove(It.IsAny<User>()), Times.Once);

            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Test]
        public async Task TogleActivity_ShouldToggleUserActivity()
        {
            // Arrange
            var user = new User { Id = "1", Active = true };
            _mockContext.Setup(c => c.Users.FindAsync("1")).ReturnsAsync(user);

            // Act
            var result = await _controller.TogleActivity("1");

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.IsFalse(user.Active);  // Check if the activity was toggled
        }
    }
}
