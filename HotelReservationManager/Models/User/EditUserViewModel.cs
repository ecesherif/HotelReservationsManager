using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationManager.Models.User
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

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
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
