using System.Diagnostics;
using HotelReservationManager.Models;
using HotelReservationManager.Models.Client;
using HotelReservationManager.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservationManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult ChooseRegisterType()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");  // Redirect to homepage if already logged in
            }

            return View();
        }
        [AllowAnonymous]
        public IActionResult RegisterUser()
        {
            return View("CreateUser", new CreateUserViewModel());
        }

        [AllowAnonymous]
        public IActionResult RegisterClient()
        {
            return View("CreateClient", new CreateClientViewModel());
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
