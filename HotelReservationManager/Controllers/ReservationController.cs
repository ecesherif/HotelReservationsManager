using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelReservationManager.Data;
using HotelReservationManager.Data.Models;
using HotelReservationManager.Models.Reservation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservationManager.Controllers
{
    [Authorize(Roles = "Amdin,Employee")]
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReservationController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservation
        public async Task<IActionResult> Index()
        {
            var reservations = await _context.Reservations.ToListAsync();
            return View(reservations);
        }

        // GET: Reservation/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FirstOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservation/Create
        public async Task<IActionResult> Create()
        {
            await _context.UpdateRooms();
            var reservationVM = new CreateReservationViewModel
            {
                AvaiableRooms = await _context.Rooms.OrderBy(x => x.Number).ToListAsync(),
                AvaiableClients = await _context.Clients.OrderBy(x => x.FirstName).ThenBy(x => x.FirstName).ToListAsync(),
                CheckInTime = DateTime.Now,
                CheckOutTime = DateTime.Now
            };
            return View(reservationVM);
        }


        // POST: Reservation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CheckInTime,CheckOutTime,Breakfast,AllInclusive,Id,RoomId,ClientIds")] CreateReservationViewModel reservationVM)
        {
            if (reservationVM.CheckOutTime < reservationVM.CheckInTime)
            {
                ModelState.AddModelError(string.Empty, "The check-out cannot be before the check-in");
            }

            var selectedRoom = await _context.Rooms.FindAsync(reservationVM.RoomId);
            if (!await _context.IsRoomFreeInPeriod(selectedRoom, reservationVM.CheckInTime, reservationVM.CheckOutTime))
            {
                ModelState.AddModelError("RoomId", "The selected room is not free during the entire period selected");
            }
            if(reservationVM.ClientIds != null) 
            {
                if (reservationVM.ClientIds.Count == 0)
                {
                    ModelState.AddModelError("ClientIds", "Select clients");
                }
                if (selectedRoom.Capacity < reservationVM.ClientIds.Count)
                {
                    ModelState.AddModelError("RoomId", "The selected room doesn't have enough capacity for the clients");
                }
            }
            else
            {
                ModelState.AddModelError("ClientIds", "Select clients");
            }

            if (ModelState.IsValid)
            {
                var currentUser = await _context.Users.FindAsync(_userManager.GetUserId(User));
                if (currentUser == null)
                {
                    return Unauthorized();
                }


                var reservation = new Reservation
                {
                    Id = Guid.NewGuid().ToString(),
                    AllInclusive = reservationVM.AllInclusive,
                    Breakfast = reservationVM.Breakfast,
                    CheckInTime = reservationVM.CheckInTime,
                    CheckOutTime = reservationVM.CheckOutTime,
                    Room = selectedRoom,
                    Creator = currentUser,
                    TotalPrice = 0
                };
                reservation.ClientReservations = reservationVM.ClientIds.Select(x => _context.Clients.Find(x))
                    .Select(c => new ClientReservation { Client = c, Reservation = reservation }).ToList();

                double price = 0;
                foreach (var client in reservation.ClientReservations.Select(x => x.Client))
                {
                    price += (client.Mature) ? reservation.Room.Price : reservation.Room.PriceChildren;
                }
                reservation.TotalPrice = price;

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            reservationVM.AvaiableRooms = await _context.Rooms.OrderBy(x => x.Number).ToListAsync();
            reservationVM.AvaiableClients = await _context.Clients.OrderBy(x => x.FirstName).ThenBy(x => x.FirstName).ToListAsync();
            return View(reservationVM);
        }

        // GET: Reservation/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FirstOrDefaultAsync();
            if (reservation == null)
            {
                return NotFound();
            }

            await _context.UpdateRooms();

            var reservationVM = new EditReservationViewModel
            {
                AllInclusive = reservation.AllInclusive,
                Breakfast = reservation.Breakfast,
                CheckInTime = reservation.CheckInTime,
                CheckOutTime = reservation.CheckOutTime,
                ClientIds = reservation.ClientReservations.Select(x => x.Client.Id).ToList(),
                RoomId = reservation.Room.Id,
                Id = reservation.Id,
            };
            reservationVM.AvaiableRooms = await _context.Rooms.OrderBy(x => x.Number).ToListAsync();
            reservationVM.AvaiableClients = await _context.Clients.OrderBy(x => x.FirstName).ThenBy(x => x.FirstName).ToListAsync();
            return View(reservationVM);
        }

        // POST: Reservation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CheckInTime,CheckOutTime,Breakfast,AllInclusive,Id,RoomId,ClientIds")] EditReservationViewModel reservationVM)
        {
            if (reservationVM.CheckOutTime < reservationVM.CheckInTime)
            {
                ModelState.AddModelError(string.Empty, "The check-out cannot be before the check-in");
            }

            var selectedRoom = await _context.Rooms.FindAsync(reservationVM.RoomId);
            if (!await _context.IsRoomFreeInPeriod(selectedRoom, reservationVM.CheckInTime, reservationVM.CheckOutTime, reservationVM.Id))
            {
                ModelState.AddModelError("RoomId", "The selected room is not free during the entire period selected");
            }
            if (reservationVM.ClientIds != null)
            {
                if (reservationVM.ClientIds.Count == 0)
                {
                    ModelState.AddModelError("ClientIds", "Select clients");
                }
                if (selectedRoom.Capacity < reservationVM.ClientIds.Count)
                {
                    ModelState.AddModelError("RoomId", "The selected room doesn't have enough capacity for the clients");
                }
            }
            else
            {
                ModelState.AddModelError("ClientIds", "Select clients");
            }

            if (ModelState.IsValid)
            {
                var currentUser = await _context.Users.FindAsync(_userManager.GetUserId(User));
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                var reservation = await _context.Reservations.Where(x => x.Id == reservationVM.Id).FirstOrDefaultAsync();
                foreach (var item in reservation.ClientReservations)
                {
                    _context.Remove(item);
                }
                try
                {
                    reservation.AllInclusive = reservationVM.AllInclusive;
                    reservation.Breakfast = reservationVM.Breakfast;
                    reservation.CheckInTime = reservationVM.CheckInTime;
                    reservation.CheckOutTime = reservationVM.CheckOutTime;
                    reservation.Creator = currentUser;
                    reservation.ClientReservations = reservationVM.ClientIds.Select(x => _context.Clients.Find(x))
                        .Select(c => new ClientReservation { Client = c, Reservation = reservation }).ToList();
                    reservation.Room = selectedRoom;
                    double price = 0;
                    foreach (var client in reservation.ClientReservations.Select(x => x.Client))
                    {
                        price += (client.Mature) ? reservation.Room.Price : reservation.Room.PriceChildren;
                    }
                    reservation.TotalPrice = price;
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservationVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            reservationVM.AvaiableRooms = await _context.Rooms.OrderBy(x => x.Number).ToListAsync();
            reservationVM.AvaiableClients = await _context.Clients.OrderBy(x => x.FirstName).ThenBy(x => x.FirstName).ToListAsync();
            return View(reservationVM);
        }

        // GET: Reservation/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var reservation = await _context.Reservations.Where(x => x.Id == id).FirstOrDefaultAsync();
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(string id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
