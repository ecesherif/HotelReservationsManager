using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelReservationManager.Data;
using HotelReservationManager.Data.Models;
using HotelReservationManager.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservationManager.Controllers
{

    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,SecondName,LastName,EGN,UserName,Email,PhoneNumber,Password,ConfirmPassword")] CreateUserViewModel userVM)
        {
            foreach (var item in userVM.EGN)
            {
                if (item < '0' || item > '9')
                {
                    ModelState.AddModelError("EGN", "The EGN mush have only digits");
                    goto Cont;
                }
            }
            
            if (await _context.Users.Where(x => x.EGN == userVM.EGN).CountAsync() != 0)
            {
                ModelState.AddModelError("EGN", "User with this EGN exists");
            }
        Cont:
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = userVM.UserName,
                    Email = userVM.Email,
                    EGN = userVM.EGN,
                    FirstName = userVM.FirstName,
                    SecondName = userVM.SecondName,
                    LastName = userVM.LastName,
                    PhoneNumber = userVM.PhoneNumber,
                    Active = true,
                    HireTime = DateTime.Now,
                };
                var result = await _userManager.CreateAsync(user, userVM.Password);
                if (result.Succeeded)
                {
                    if (user.UserName.Contains("Admin"))
                    { await _userManager.AddToRoleAsync(user, "Admin"); }
                    else
                    { await _userManager.AddToRoleAsync(user, "Employee"); }
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(userVM);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var userVM = new EditUserViewModel
            {
                EGN = user.EGN,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                LastName = user.LastName,
                FirstName = user.LastName,
                SecondName = user.SecondName,
                HireTime = user.HireTime,
                FireTime = user.FireTime,
                Active = user.Active,
                Id = user.Id
            };
            return View(userVM);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("FirstName,SecondName,LastName,EGN,Id,UserName,Email,PhoneNumber,HireTime,FireTime,Active")] EditUserViewModel userVM)
        {
            if (id != userVM.Id)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Check if EGN has changed
            if (user.EGN != userVM.EGN)
            {
                // Validate that EGN contains only digits
                foreach (var item in userVM.EGN)
                {
                    if (item < '0' || item > '9')
                    {
                        ModelState.AddModelError("EGN", "The EGN must have only digits.");
                        return View(userVM); // return the view with the error
                    }
                }

                // Validate if the EGN already exists in the database
                if (await _context.Users.Where(x => x.EGN == userVM.EGN).CountAsync() != 0)
                {
                    ModelState.AddModelError("EGN", "User with this EGN exists.");
                    return View(userVM); // return the view with the error
                }
            }

            if (ModelState.IsValid)
            {
                user.FirstName = userVM.FirstName;
                user.SecondName = userVM.SecondName;
                user.LastName = userVM.LastName;
                user.EGN = userVM.EGN;
                user.UserName = userVM.UserName;
                user.Email = userVM.Email;
                user.PhoneNumber = userVM.PhoneNumber;
                user.HireTime = userVM.HireTime;
                user.FireTime = userVM.FireTime;
                user.Active = userVM.Active;

                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(userVM);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> TogleActivity(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user.FireTime.HasValue)
            {
                // User is deactivated, handle reactivation logic
                ModelState.AddModelError("", "This user is already deactivated due to FireTime. Reactivate if necessary.");
                return RedirectToAction(nameof(Index));
            }
            if (user.Active)
            {
                await _userManager.RemoveFromRoleAsync(user, "Employee");
                await _userManager.UpdateSecurityStampAsync(user);
                user.Active = false;
                user.FireTime = DateTime.Now;
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Employee");
                user.Active = true;
                user.FireTime = null;
                user.HireTime = DateTime.Now;
            }
            _context.Update(user);
            await _context.SaveChangesAsync();
            await _userManager.UpdateSecurityStampAsync(user);
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
