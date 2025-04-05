using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using HotelReservationManager.Controllers;
using HotelReservationManager.Data;
using HotelReservationManager.Data.Models;
using HotelReservationManager.Models.Client;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace HotelReservationManager.Tests
{
    public class ClientControllerTests
    {
        private ClientController _controller;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            // Create the in-memory database context
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("HotelReservationTestDB")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new ClientController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose the controller and context after each test
            _controller.Dispose();
            _context.Dispose();
        }

        [Test]
        public async Task Create_ShouldReturnView_WhenModelIsInvalid()
        {
            // Arrange
            var clientVM = new CreateClientViewModel
            {
                FirstName = "", // Invalid data (empty name)
                LastName = "Doe",
                PhoneNumber = "123456789",
                Email = "email@example.com",
                Mature = true
            };

            _controller.ModelState.AddModelError("FirstName", "First name is required.");

            // Act
            var result = await _controller.Create(clientVM);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.AreEqual(viewResult.Model, clientVM);
        }

        [Test]
        public async Task Create_ShouldRedirectToIndex_WhenModelIsValid()
        {
            // Arrange
            var clientVM = new CreateClientViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "123456789",
                Email = "john.doe@example.com",
                Mature = true
            };

            // Act
            var result = await _controller.Create(clientVM);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.AreEqual(redirectResult.ActionName, "Index");
        }

        [Test]
        public async Task Edit_ShouldReturnNotFound_WhenClientDoesNotExist()
        {
            // Arrange
            var invalidId = "non-existent-id";

            // Act
            var result = await _controller.Edit(invalidId);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(notFoundResult.StatusCode, 404);
        }

        [Test]
        public async Task DeleteConfirmed_ShouldRemoveClientAndRedirect()
        {
            // Arrange
            var client = new Client
            {
                Id = "1",
                FirstName = "John",
                LastName = "Smith",
                PhoneNumber = "111222333",
                Email = "john.smith@example.com",
                Mature = true
            };
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteConfirmed("1");

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.AreEqual(redirectResult.ActionName, "Index");

            var clientFromDb = await _context.Clients.FindAsync("1");
            Assert.Null(clientFromDb); // Client should be removed from the database
        }

        [Test]
        public async Task Details_ShouldReturnNotFound_WhenClientDoesNotExist()
        {
            // Arrange
            var invalidId = "non-existent-id";

            // Act
            var result = await _controller.Details(invalidId);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(notFoundResult.StatusCode, 404);
        }

    }
}
