using HotelReservationManager.Data.Models;
using HotelReservationManager.Data;
using HotelReservationManager.Models.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationManager.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Client
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.ToListAsync());
        }

        // GET: Client/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            var clientVM = new DetailsClientViewModel
            {
                Reservations = client.ClientReservations.Select(x => x.Reservation).ToList(),
                Email = client.Email,
                FirstName = client.FirstName,
                Id = client.Id.ToString(),
                LastName = client.LastName,
                Mature = client.Mature,
                PhoneNumber = client.PhoneNumber
            };

            return View(clientVM);
        }

        // GET: Client/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,PhoneNumber,Email,Mature")] CreateClientViewModel clientVM)
        {
            if (ModelState.IsValid)
            {
                var client = new Client
                {
                    Email = clientVM.Email,
                    FirstName = clientVM.FirstName,
                    Id = int.Parse(Guid.NewGuid().ToString()),
                    LastName = clientVM.LastName,
                    Mature = clientVM.Mature,
                    PhoneNumber = clientVM.PhoneNumber
                };
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clientVM);
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            var clientVM = new EditClientViewModel
            {
                Email = client.Email,
                FirstName = client.FirstName,
                Id = client.Id.ToString(),
                LastName = client.LastName,
                Mature = client.Mature,
                PhoneNumber = client.PhoneNumber
            };
            return View(clientVM);
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("FirstName,LastName,PhoneNumber,Email,Mature,Id")] EditClientViewModel clientVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var client = new Client
                    {
                        Email = clientVM.Email,
                        FirstName = clientVM.FirstName,
                        Id = int.Parse(clientVM.Id),
                        LastName = clientVM.LastName,
                        Mature = clientVM.Mature,
                        PhoneNumber = clientVM.PhoneNumber
                    };
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(int.Parse(clientVM.Id)))
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
            return View(clientVM);
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
