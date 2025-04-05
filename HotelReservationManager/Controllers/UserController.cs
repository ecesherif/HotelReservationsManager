using HotelReservationManager.Data.Models;
using HotelReservationManager.Data;
using HotelReservationManager.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelReservationManager.Models.Client;

namespace HotelReservationManager.Controllers
{
    
    public class UserController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private bool CheckEGN(string EGN)
        {
            var a = new int[] { 2, 4, 8, 5, 10, 9, 7, 3, 6 };
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (EGN[i] - '0') * a[i];
            }
            sum %= 11;
            if (sum == 10)
                sum = 0;
            return EGN[9] == (sum + '0');
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View("IndexUser",await _context.Users.ToListAsync());
        }

        // GET: User/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,SecondName,LastName,EGN,UserName,Email,PhoneNumber,Password,ConfirmPassword")] CreateUserViewModel userVM)
        {
            foreach (var item in userVM.EGN)
            {
                if (item < '0' || item > '9')
                {
                    ModelState.AddModelError("EGN", "ЕГН-то трябва да се състои само от цифри.");
                    return View(userVM);
                }
            }
            if (!CheckEGN(userVM.EGN))
            {
                ModelState.AddModelError("EGN", "Невалидно ЕГН.");
            }
            if (await _context.Users.AnyAsync(x => x.EGN == userVM.EGN))
            {
                ModelState.AddModelError("EGN", "Има потребител с това ЕГН.");
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
                    HireDate = DateTime.Now,
                };
                var result = await _userManager.CreateAsync(user, userVM.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Employee");
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(userVM);
        }
        // GET: User/Details/5
        public async Task<IActionResult> Details(string id)
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

            var userVM = new DetailsUserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                LastName = user.LastName,
                EGN = user.EGN,
                PhoneNumber = user.PhoneNumber
            };

            return View("DetailUser", userVM);
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
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                Id = user.Id
            };
            return View("EditUser", userVM);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("FirstName,SecondName,LastName,EGN,Id,UserName,Email,PhoneNumber")] EditUserViewModel userVM)
        {
            foreach (var item in userVM.EGN)
            {
                if (item < '0' || item > '9')
                {
                    ModelState.AddModelError("EGN", "ЕГН-то трябва да се състои само от цифри.");
                    return View(userVM);
                }
            }
            if (!CheckEGN(userVM.EGN))
            {
                ModelState.AddModelError("EGN", "Невалидно ЕГН.");
            }
            if (await _context.Users.AnyAsync(x => x.EGN == userVM.EGN))
            {
                ModelState.AddModelError("EGN", "Има потребител с това ЕГН.");
            }
        Cont:
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users.FindAsync(userVM.Id);
                    user.FirstName = userVM.FirstName;
                    user.SecondName = userVM.SecondName;
                    user.LastName = userVM.LastName;
                    user.EGN = userVM.EGN;
                    user.PhoneNumber = userVM.PhoneNumber;
                    user.UserName = userVM.UserName;
                    user.Email = userVM.Email;

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    if (!UserExists(userVM.Id))
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
            return View("EditUser", userVM);
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

            return View("DeleteUser", user);
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
            if (user.Active)
            {
                await _userManager.RemoveFromRoleAsync(user, "Employee");
                await _userManager.UpdateSecurityStampAsync(user);
                user.Active = false;
                user.FireDate = DateTime.Now;
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Employee");
                user.Active = true;
                user.FireDate = null;
                user.HireDate = DateTime.Now;
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
