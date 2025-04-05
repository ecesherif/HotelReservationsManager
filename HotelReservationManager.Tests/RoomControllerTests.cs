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
using HotelReservationManager.Models.Room;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace HotelReservationManager.Tests
{
    public class RoomControllerTests
    {
        private RoomController _controller;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            // Create the in-memory database context
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("HotelReservationTestDB")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new RoomController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose the controller after each test
            _controller.Dispose();
            _context.Dispose();
        }

        [Test]
        public async Task Create_ShouldReturnView_WhenModelIsInvalid()
        {
            // Arrange
            var roomVM = new CreateRoomViewModel
            {
                Price = -10,
                PriceChildren = -5,
                Number = 101.ToString()
            };
            _controller.ModelState.AddModelError("Price", "The price must be positive");
            _controller.ModelState.AddModelError("PriceChildren", "The price for children must be positive");

            // Act
            var result = await _controller.Create(roomVM);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.AreEqual(viewResult.Model, roomVM);
        }

        [Test]
        public async Task Create_ShouldRedirectToIndex_WhenModelIsValid()
        {
            // Arrange
            var roomVM = new CreateRoomViewModel
            {
                Price = 100,
                PriceChildren = 50,
                Number = 103.ToString(),
                Capacity = 2,
                Type = Enum.Parse<RoomType>("TwoBeds")  // This is a valid enum value
            };

            // Act
            var result = await _controller.Create(roomVM);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.AreEqual(redirectResult.ActionName, "Index");
        }

        [Test]
        public async Task Edit_ShouldReturnNotFound_WhenRoomDoesNotExist()
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
        public async Task DeleteConfirmed_ShouldRemoveRoomAndRedirect()
        {
            // Arrange
            var room = new Room
            {
                Id = "1",
                Number = 101.ToString(),
                Capacity = 2,
                Type = Enum.Parse<RoomType>("TwoBeds"),
                Price = 100
            };

            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteConfirmed("1");

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.AreEqual(redirectResult.ActionName, "Index");
        }

    }
}
