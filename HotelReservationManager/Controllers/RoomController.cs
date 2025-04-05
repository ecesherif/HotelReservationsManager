using HotelReservationManager.Data.Models;
using HotelReservationManager.Data;
using HotelReservationManager.Models.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationManager.Controllers
{
    
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            await UpdateRooms();
            var rooms = await _context.Rooms.ToListAsync();
            return View("IndexRoom", rooms);  // Specify the custom view name
        }



        // GET: Rooms/Create

        public IActionResult Create()
        {
            return View("CreateRoom");
        }

        public async Task<bool> IsRoomFreeInPeriod(Room room, DateTime begin, DateTime end, string currReservId = null)
        {
            var reservs = await _context.Reservations.Where(x => x.Room.Id == room.Id && (currReservId == null || x.Id != int.Parse(currReservId))).ToListAsync();
            foreach (var reserv in reservs)
            {
                if ((begin >= reserv.CheckInTime && begin <= reserv.CheckOutTime) ||
                    (end >= reserv.CheckInTime && end <= reserv.CheckOutTime) ||
                    (begin <= reserv.CheckInTime && end >= reserv.CheckOutTime))
                {
                    return false;
                }
            }
            return true;
        }

        public async Task UpdateRoom(Room room)
        {
            room.Free = true;
            var reservs = await _context.Reservations.Where(x => x.Room.Id == room.Id).ToListAsync();
            foreach (var reserv in reservs)
            {
                if (reserv.CheckInTime.Date <= DateTime.Now.Date && DateTime.Now.Date <= reserv.CheckOutTime.Date)
                {
                    room.Free = false;
                    _context.Update(room);
                }
            }
        }

        public async Task UpdateRooms()
        {
            var rooms = await _context.Rooms.ToListAsync();
            foreach (var room in rooms)
            {
                await UpdateRoom(room);
            }
            await _context.SaveChangesAsync();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create([Bind("Capacity,Type,Free,Price,PriceChildren,Number")] CreateRoomViewModel roomVM)
        {
            if (roomVM.PriceAdult < 0)
            {
                ModelState.AddModelError("Price", "The price must be positive");
            }
            if (roomVM.PriceKid < 0)
            {
                ModelState.AddModelError("PriceChildren", "The price for children must be positive");
            }
            if (await _context.Rooms.AnyAsync(x => x.Number == roomVM.Number))
            {
                ModelState.AddModelError("Number", "Room with the same number already exists");
            }
            if (ModelState.IsValid)
            {
                var room = new Room
                {
                    Id = int.Parse(Guid.NewGuid().ToString()),
                    Capacity = roomVM.Capacity,
                    Type = roomVM.Type,
                    PriceAdult = roomVM.PriceAdult,
                    PriceKid = roomVM.PriceKid,
                    Number = roomVM.Number,
                    Free = true
                };
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View( roomVM);
        }

        // GET: Rooms/Edit/5
        
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            await UpdateRoom(room);
            var roomVM = new EditRoomViewModel
            {
                Capacity = room.Capacity,
                Free = room.Free,
                Id = room.Id.ToString(),
                Number = room.Number,
                PriceAdult = room.PriceAdult,
                PriceKid = room.PriceKid,
                Type = room.Type
            };
            return View("EditRoom", roomVM);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit([Bind("Capacity,Type,Free,Price,PriceChildren,Number,Id")] EditRoomViewModel roomVM)
        {
            if (roomVM.PriceAdult < 0)
            {
                ModelState.AddModelError("Price", "The price must be positive");
            }
            if (roomVM.PriceKid < 0)
            {
                ModelState.AddModelError("PriceChildren", "The price for children must be positive");
            }
            var room = await _context.Rooms.FindAsync(roomVM.Id);
            if (room == null)
            {
                return NotFound();
            }
            if (await _context.Rooms.AnyAsync(x => x.Number == roomVM.Number && x.Id != int.Parse(roomVM.Id)))
            {
                ModelState.AddModelError("Number", "Room with the same number already exists");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    room.Capacity = roomVM.Capacity;
                    room.Type = roomVM.Type;
                    room.Free = roomVM.Free;
                    room.PriceAdult = roomVM.PriceAdult;
                    room.PriceKid = roomVM.PriceKid;
                    room.Number = roomVM.Number;

                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    if (!await _context.Rooms.AnyAsync(e => e.Id == int.Parse(roomVM.Id)))
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
            return View(roomVM);
        }

        // GET: Rooms/Delete/5
        
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            return View("DeleteRoom", room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var room = await _context.Rooms.FindAsync(id);
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
