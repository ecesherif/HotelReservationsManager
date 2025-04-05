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
using HotelReservationManager.Models.Reservation;
using Microsoft.AspNetCore.Identity;

namespace HotelReservationManager.Tests
{
    public class ReservationControllerTests
    {
        private ReservationController _controller;
        private ApplicationDbContext _context;
        private Mock<UserManager<User>> _mockUserManager;

        [SetUp]
        public void Setup()
        {
            // Create the in-memory database context
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("HotelReservationTestDB")
                .Options;

            _context = new ApplicationDbContext(options);

            // Mock UserManager
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);

            _controller = new ReservationController(_context, _mockUserManager.Object);
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
            var reservationVM = new CreateReservationViewModel
            {
                CheckInTime = DateTime.Now,
                CheckOutTime = DateTime.Now.AddDays(-1), // Invalid checkout time (before check-in)
                RoomId = "1", // Room ID that may not be available
                ClientIds = new System.Collections.Generic.List<string>()
            };
            _controller.ModelState.AddModelError(string.Empty, "Invalid check-out time.");

            // Act
            var result = await _controller.Create(reservationVM);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.AreEqual(viewResult.Model, reservationVM);
        }

        [Test]
        public async Task Create_ShouldRedirectToIndex_WhenModelIsValid()
        {
            // Arrange: Ensure you provide valid values for Room and Client
            var room = new Room
            {
                Id = "1",
                Number = 101.ToString(),
                Price = 100,
                PriceChildren = 50,
                Capacity = 2
            };

            var client = new Client
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com", // Ensure Email is valid
                PhoneNumber = "123-456-7890",  // Ensure PhoneNumber is valid
                Mature = true
            };

            // Add the room and client to the database context
            await _context.Rooms.AddAsync(room);
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();

            // Ensure reservationVM is populated correctly
            var reservationVM = new CreateReservationViewModel
            {
                CheckInTime = DateTime.Now.AddDays(1),
                CheckOutTime = DateTime.Now.AddDays(5),
                RoomId = room.Id,            // Ensure RoomId is set
                ClientIds = new List<string> { client.Id }  // Ensure ClientIds is not empty
            };

            // Act: Call the Create action in the controller
            var result = await _controller.Create(reservationVM);

            // Assert: Check that the result is a redirect to the Index action
            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.AreEqual(redirectResult.ActionName, "Index");
        }


        [Test]
        public async Task Edit_ShouldReturnNotFound_WhenReservationDoesNotExist()
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
        public async Task Edit_ShouldReturnView_WhenModelStateIsValid()
        {
            // Arrange
            var room = new Room
            {
                Id = "1",
                Number = 101.ToString(),
                Price = 100,
                PriceChildren = 50,
                Capacity = 2
            };
            var client = new Client { Id = "1", FirstName = "John", LastName = "Doe", Mature = true };
            var reservation = new Reservation
            {
                Id = "1",
                Room = room,
                CheckInTime = DateTime.Now.AddDays(1),
                CheckOutTime = DateTime.Now.AddDays(5),
                ClientReservations = new System.Collections.Generic.List<ClientReservation>
                {
                    new ClientReservation { ClientId = client.Id, Client = client }
                },
                TotalPrice = 100
            };

            await _context.Rooms.AddAsync(room);
            await _context.Clients.AddAsync(client);
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            var reservationVM = new EditReservationViewModel
            {
                Id = reservation.Id,
                CheckInTime = DateTime.Now.AddDays(1),
                CheckOutTime = DateTime.Now.AddDays(5),
                RoomId = room.Id,
                ClientIds = new System.Collections.Generic.List<string> { client.Id }
            };

            // Act
            var result = await _controller.Edit(reservationVM.Id);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.AreEqual(viewResult.Model, reservationVM);
        }

        [Test]
        public async Task DeleteConfirmed_ShouldRemoveReservationAndRedirect()
        {
            // Arrange
            var room = new Room
            {
                Id = "1",
                Number = 101.ToString(),
                Price = 100,
                PriceChildren = 50,
                Capacity = 2
            };
            var client = new Client { Id = "1", FirstName = "John", LastName = "Doe", Mature = true };
            var reservation = new Reservation
            {
                Id = "1",
                Room = room,
                CheckInTime = DateTime.Now.AddDays(1),
                CheckOutTime = DateTime.Now.AddDays(5),
                ClientReservations = new System.Collections.Generic.List<ClientReservation>
                {
                    new ClientReservation { ClientId = client.Id, Client = client }
                },
                TotalPrice = 100
            };

            await _context.Rooms.AddAsync(room);
            await _context.Clients.AddAsync(client);
            await _context.Reservations.AddAsync(reservation);
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
