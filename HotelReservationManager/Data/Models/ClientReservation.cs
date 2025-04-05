using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelReservationManager.Data.Models
{
    public class ClientReservation 
    {
        [Required]
        [Display(Name = "Id")]
        public string Id { get; set; }
        public string ClientId { get; set; }
        public virtual Client Client { get; set; }

        public string ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}
