using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using HotelReservationManager.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace HotelReservationManager.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(50)]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "Second name")]
            public string SecondName { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [Required]
            [StringLength(10, MinimumLength = 10, ErrorMessage = "The EGN must be 10 digits")]
            public string EGN { get; set; }

            [Required]
            [Phone]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"((?:\+|00)[17](?: |\-)?|(?:\+|00)[1-9]\d{0,2}(?: |\-)?|(?:\+|00)1\-\d{3}(?: |\-)?)?(0\d|\([0-9]{3}\)|[1-9]{0,3})(?:((?: |\-)[0-9]{2}){4}|((?:[0-9]{2}){4})|((?: |\-)[0-9]{3}(?: |\-)[0-9]{4})|([0-9]{7}))", ErrorMessage = "Invalid phone number")]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        public IActionResult OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (_userManager.Users.Count() > 0)
                return Unauthorized();
            return Page();
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

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (_userManager.Users.Count() > 0)
                return Unauthorized();
            returnUrl = returnUrl ?? Url.Content("~/");

            foreach (var item in Input.EGN)
            {
                if (item < '0' || item > '9')
                {
                    ModelState.AddModelError("EGN", "The EGN mush have only digits");
                    goto Cont;
                }
            }
            if (!CheckEGN(Input.EGN))
            {
                ModelState.AddModelError("EGN", "Invalid EGN");
            }
        Cont:
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = Input.UserName,
                    Email = Input.Email,
                    EGN = Input.EGN,
                    FirstName = Input.FirstName,
                    SecondName = Input.SecondName,
                    LastName = Input.LastName,
                    PhoneNumber = Input.PhoneNumber,
                    Active = true,
                    HireTime = DateTime.Now
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (_userManager.Users.Count() == 1)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                        await _roleManager.CreateAsync(new IdentityRole("Employee"));
                        await _userManager.AddToRoleAsync(user, "Admin");
                        await _userManager.AddToRoleAsync(user, "Employee");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Employee");
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
