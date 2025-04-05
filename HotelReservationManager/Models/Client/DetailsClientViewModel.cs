using HotelReservationManager.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationManager.Models.Client
{
    public class DetailsClientViewModel
    {
        public string Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Adult")]
        public bool Mature { get; set; }

        public virtual List<Data.Models.Reservation> Reservations { get; set; }
    }
}
