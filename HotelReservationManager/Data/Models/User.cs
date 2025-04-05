using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationManager.Data.Models
{
    public class User : IdentityUser<string>
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone number")]
        public override string PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Username")]
        public override string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string SecondName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "The EGN must be 10 digits")]
        public string EGN { get; set; }

        [Required]
        [Display(Name = "Enabled on")]
        public DateTime HireTime { get; set; }

        [Required]
        public bool Active { get; set; }

        [Display(Name = "Disabled on")]
        public DateTime? FireTime { get; set; }
    }
}